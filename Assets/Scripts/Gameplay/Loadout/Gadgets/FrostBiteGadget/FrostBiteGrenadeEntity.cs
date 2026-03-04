using Ami.BroAudio;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Database;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Abilities;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.FrostBiteGadget
{
    public class FrostBiteGrenadeEntity : NetworkBehaviour, IPoolInstanceListener
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private NetworkObject no;
        [SerializeField, Self] private TrailRenderer trail;
        [SerializeField] private Collider collider;
        
        private IExplosionStrategy strategy;
        private FrostBiteGrenadeData Data;
        private FrostBiteGrenade frostBiteGrenade;
        private NetworkObject vfx;
        private float time;
        private bool isDetonated;

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
        
        public void Initialize(FrostBiteGrenadeData data, FrostBiteGrenade grenade)
        {
            Data = data;
            InitializeRpc(Data.ID);
            strategy = new CryoExplosion(Data.DamageData, Data.SlowDuration, Data.SlowPercentage);
            frostBiteGrenade = grenade;
            strategy.OnExploded += OnExploded;
        }

        [Rpc(SendTo.Everyone)]
        private void InitializeRpc(string dataId)
        {
            Debug.Log($"Database null ? {GameDatabase.Global == null}");
            Debug.Log($"Database assets count : {GameDatabase.Global?.DatabaseAssets.Length}");
            if (GameDatabase.Global.TryGetAssetByID(dataId, out FrostBiteGrenadeData asset))
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
            if (frostBiteGrenade == null) return;
            if (!frostBiteGrenade.IsCasting)
            {
                transform.position = frostBiteGrenade.Caster.CastAnchor.transform.position;
            }
        }

        public void OnExploded(bool terminated)
        {
            
            if (Data.ExplosionEffect != null)
            {
                vfx = frostBiteGrenade.spawnManager.InstantiateAndSpawn(Data.ExplosionEffect,
                    frostBiteGrenade.player.ClientID,
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
                End();
            }
        }
        
        [Rpc(SendTo.Everyone)]
        private void PlayVfxRpc(ulong networkObjectId)
        {
            if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects
                    .TryGetValue(networkObjectId, out NetworkObject netObj))
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
        
        private void DestroyVfx()
        {
            if(!IsOwner) return;
            if(vfx != null) vfx.Despawn();
        }
        
        private void End()
        {
            if(!IsOwner) return;
            frostBiteGrenade.End();
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