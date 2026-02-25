using Ami.BroAudio;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Abilities;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.FrostBiteGadget
{
    public class FrostBiteGrenadeEntity : MonoBehaviour, IPoolInstanceListener
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private NetworkObject no;
        [SerializeField, Self] private TrailRenderer trail;
        [SerializeField] private Collider collider;

        private IExplosionStrategy strategy;

        private FrostBiteGrenadeData Data;
        private FrostBiteGrenade frostBiteGrenade;
        private float time;
        private bool isDetonated;
        
        private NetworkObject vfx;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void FreezeGrenade(bool value)
        {
            rb.isKinematic = value;
            collider.isTrigger = value;
            trail.enabled = !value;
        }
        
        public void Initialize(FrostBiteGrenadeData data, FrostBiteGrenade grenade)
        {
            Data = data;
            strategy = new CryoExplosion(Data.DamageData, Data.SlowDuration, Data.SlowPercentage);
            frostBiteGrenade = grenade;
            strategy.OnExploded += OnExploded;
        }

        public void Cast(Vector3 direction)
        {
            FreezeGrenade(false);
            rb.AddForce(Vector3.up * 0.5f + direction * Data.ThrowForce * Time.deltaTime, ForceMode.Impulse);
        }
        
        public void Tick(float deltaTime)
        {
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
            if (frostBiteGrenade == null) return;
            if (!frostBiteGrenade.IsCasting)
            {
                transform.position = frostBiteGrenade.Caster.CastAnchor.transform.position;
            }
        }

        public void OnExploded(bool terminated)
        {
            BroAudio.Play(Data.SoundID);
            
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
                if (vfx.TryGetComponent(out ParticleSystem ps))
                {
                    ps.Play();
                    Invoke(nameof(DestroyVfx), ps.main.duration);
                }
            }
            
            if (terminated)
            {
                strategy.OnExploded -= OnExploded;
                End();
            }
        }

        private void DestroyVfx()
        {
            if(vfx != null) vfx.Despawn();
        }
        private void End()
        {
            frostBiteGrenade.End();
            no.Despawn();
        }

        public void Discard()
        {
            no.Despawn();
        }

        public IPool Pool { get; }
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