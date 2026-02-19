using Ami.BroAudio;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Abilities;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.BurstGadget
{
    public class BurstGrenadeEntity : MonoBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private TrailRenderer trail;
        [SerializeField] private Collider collider;

        
        private IExplosionStrategy strategy;

        private BurstGrenadeData data;
        private BurstGrenade burstGrenade;
        private float time;
        private bool isDetonated;

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
        
        public void Initialize(BurstGrenadeData data, Vector3 direction, BurstGrenade burstGrenade)
        {
            strategy = new StandardExplosion(data.DamageData);
            this.burstGrenade = burstGrenade;
            this.data = data;
            strategy.OnExploded += OnExploded;
            FreezeGrenade(false);
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);
        }
        
        public void Tick(float deltaTime)
        {
            if (time < data.ExplosionDelay)
            {
                time += deltaTime;
            }
            else if (!isDetonated)
            {
                strategy.Explode(() =>
                {
                    Collider[] colliders = Physics.OverlapSphere(
                        transform.position,
                        data.ExplosionRadius,
                        GameMetrics.Global.HittableLayers,
                        QueryTriggerInteraction.Collide);

                    return colliders;
                });
                
                isDetonated = true;
            }
        }
        
        private void OnExploded(bool terminated)
        {
            BroAudio.Play(data.SoundID);
            if (data.ExplosionEffect != null)
            {
                ParticleSystem ps = Instantiate(data.ExplosionEffect, transform.position, Quaternion.identity);
                Destroy(ps.gameObject, ps.main.duration);
            }
            if (terminated)
            {
                strategy.OnExploded -= OnExploded;
                End();
            }
        }
        
        private void End()
        {
            burstGrenade.End();
            Destroy(gameObject);
        }
    }
}