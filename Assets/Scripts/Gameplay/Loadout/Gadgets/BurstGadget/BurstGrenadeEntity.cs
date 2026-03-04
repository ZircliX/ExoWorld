using System.Data;
using Ami.BroAudio;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Database;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Abilities;
using OverBang.ExoWorld.Gameplay.Loadout.FrostBiteGadget;
using OverBang.ExoWorld.Gameplay.Targeting;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.BurstGadget
{
    public class BurstGrenadeEntity : NetworkBehaviour, IPoolInstanceListener
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private NetworkObject no;
        [SerializeField, Self] private TrailRenderer trail;
        [SerializeField, Child] private MeshRenderer meshRenderer;
        [SerializeField] private Collider collider;
        private BaliseVfxInitializer baliseVfxInitializer;
        
        private IExplosionStrategy strategy;
        private BurstGrenadeData Data;
        private BurstGrenade burstGrenade;
        private NetworkObject vfx;
        private NetworkObject burstZoneVfx;
        private float time;
        private float zoneTimer;
        private bool isDetonated;
        private bool isZoneEnded;
        private bool burstZoneActive;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        [Rpc(SendTo.Everyone)]
        public void FreezeGrenadeRpc(bool value)
        {
            rb.isKinematic = value;
            collider.isTrigger = value;
            trail.enabled = !value;
        }
        
        public void Initialize(BurstGrenadeData data, BurstGrenade grenade)
        {
            Data = data;
            meshRenderer.enabled = true;
            isZoneEnded = false;
            zoneTimer = 0f;
            time = 0f;
            InitializeRpc(Data.ID);
            strategy = new StandardExplosion(Data.DamageData);
            burstGrenade = grenade;
            strategy.OnExploded += OnExploded;
        }
        
        [Rpc(SendTo.Everyone)]
        private void InitializeRpc(string dataId)
        {
            if (GameDatabase.Global.TryGetAssetByID(dataId, out BurstGrenadeData asset))
            {
                Data = asset;
            }
            else
            {
                Debug.LogError($"[FrostBiteGrenadeEntity] {dataId} not found in database !");
            }
        }
        
        public void Cast(Vector3 direction)
        {
            FreezeGrenadeRpc(false);
            rb.AddForce(Vector3.up * 0.5f + direction * Data.ThrowForce * Time.deltaTime, ForceMode.Impulse);
        }
        
        
        public void Tick(float deltaTime)
        {
            if(!IsOwner) return;

            if (time < Data.ExplosionDelay)
            {
                time += deltaTime;
            }
            else if (!isDetonated)
            {
                strategy.Explode(() =>
                {
                    Collider[] colliders = Physics.OverlapSphere(
                        transform.position,
                        Data.ExplosionRadius,
                        GameMetrics.Global.HittableLayers,
                        QueryTriggerInteraction.Collide);

                    return colliders;
                });
                
                isDetonated = true;
            }
        }
        
        private void Update()
        {
            if (!IsOwner) return;
            if (burstGrenade == null) return;
            if (!burstGrenade.IsCasting)
            {
                transform.position = burstGrenade.Caster.CastAnchor.transform.position;
            }

            if (burstZoneActive)
            {
                TickBurstZone(Time.deltaTime);
            }
        }
        
        public void OnExploded(bool terminated)
        {
            if (Data.ExplosionEffect != null)
            {
                vfx = burstGrenade.spawnManager.InstantiateAndSpawn(Data.ExplosionEffect,
                    burstGrenade.player.ClientID,
                    true,
                    true,
                    false,
                    transform.position,
                    Quaternion.identity
                );
                PlayVfxRpc(vfx.NetworkObjectId);
            }
            
            if (terminated)
            {
                strategy.OnExploded -= OnExploded;
                meshRenderer.enabled = false;
                trail.enabled = false;
                StartBurstZone();
            }
        }


        private void StartBurstZone()
        {
            burstZoneActive = true;
            
            burstZoneVfx = burstGrenade.spawnManager.InstantiateAndSpawn(Data.FireZoneVfx,
                burstGrenade.player.ClientID,
                true,
                true,
                false,
                transform.position,
                Quaternion.identity
            );
            PlayZoneVfxRpc(burstZoneVfx.NetworkObjectId);
        }

        private void TickBurstZone(float deltaTime)
        {
            time += deltaTime;
            if (time >= 2f && !isZoneEnded)
            { 
                time = 0f;
                DoBurstDamageToEnemies();
            }

            zoneTimer += deltaTime;
            //Debug.Log(zoneTimer);
            if (zoneTimer >= Data.ZoneDuration)
            {
                isZoneEnded = true;
                EndBurstZone();
            }
            
        }

        private void DoBurstDamageToEnemies()
        {
            Debug.Log("DoBurstDamageToEnemies");
            Collider[] colliders = Physics.OverlapSphere(
                transform.position,
                Data.ExplosionRadius,
                GameMetrics.Global.HittableLayers,
                QueryTriggerInteraction.Collide);

            if (colliders.Length > 0)
            {
                foreach (Collider col in colliders)
                {
                    if (col.TryGetComponent(out IDamageable entity))
                    {
                        entity.TakeDamage(Data.FireDamageData.GetRuntimeDamage());
                    }
                }
            }
        }

        private void EndBurstZone()
        {
            burstZoneActive = false;
            End();
        }
        
        #region VFX
        
        [Rpc(SendTo.Everyone)]
        private void PlayVfxRpc(ulong networkObjectId)
        {
            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
            {
                Debug.LogError($"VFX with ID {networkObjectId} not found.");
                return;
            }
            
            vfx = netObj;
            if (vfx.TryGetComponent(out ParticleSystem ps))
            {
                ps.Play();
                Invoke(nameof(DestroyVfx), ps.main.duration);
            }

            BroAudio.Play(Data.SoundID);
        }
        
        [Rpc(SendTo.Everyone)]
        private void PlayZoneVfxRpc(ulong networkObjectId)
        {
            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(networkObjectId, out NetworkObject netObj))
            {
                Debug.LogError($"VFX with ID {networkObjectId} not found.");
                return;
            }
            
            burstZoneVfx = netObj;
            
            if (burstZoneVfx.TryGetComponent(out BaliseVfxInitializer initializer))
            {
                baliseVfxInitializer = initializer;
                baliseVfxInitializer.InitializeVfx(Data.ZoneDuration, Data.ExplosionRadius);
            }
            
            if (burstZoneVfx.TryGetComponent(out ParticleSystem ps))
            {
                ps.Play();
                Invoke(nameof(DestroyFireVfx), ps.main.duration);
            }
        }
        
        
        private void DestroyVfx()
        {
            if(!IsOwner) return;
            if(vfx != null) vfx.Despawn();
        }
        
        private void DestroyFireVfx()
        {
            if(!IsOwner) return;
            if(burstZoneVfx != null) burstZoneVfx.Despawn();
        }
        
        #endregion
        
        private void End()
        {
            if(!IsOwner) return;
            burstGrenade.End();
            no.Despawn();
        }

        public void Discard()
        {
            if(!IsOwner) return;
            no.Despawn();
        }

        public void OnSpawn(IPool pool)
        {
            isDetonated = false;
            time = 0f;
        }

        public void OnDespawn(IPool pool)
        {
            isDetonated = false;
            time = 0f;
        }
    }
}