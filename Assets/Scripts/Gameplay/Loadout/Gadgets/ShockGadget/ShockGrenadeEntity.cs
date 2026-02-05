using Ami.BroAudio;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Abilities;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.ShockGadget
{
    public class ShockGrenadeEntity : MonoBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField] private Collider collider;
        
        private IExplosionStrategy strategy;

        private ShockGrenadeData data;
        private ShockGrenade shockGrenade;
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
        }

        public void Initialize(ShockGrenadeData data, Vector3 direction, ShockGrenade shockGrenade)
        {
            strategy = new StandardExplosion(data.DamageData);
            this.shockGrenade = shockGrenade;
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
            
            if (terminated)
            {
                strategy.OnExploded -= OnExploded;
                End();
            }
        }

        private void End()
        {
            shockGrenade.End();
            Destroy(gameObject);
        }
    }
}