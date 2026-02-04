using Ami.BroAudio;
using KBCore.Refs;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Abilities;
using OverBang.ExoWorld.Gameplay.Loadout.ShockGadget;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.LifePulseGadget
{
    public class LifePulseEntity : MonoBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField] private Collider collider;
        
        private IExplosionStrategy strategy;

        private LifePulseData data;
        private LifePulse lifePulseGrenade;
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
        
        public void Initialize(LifePulseData data, Vector3 direction, LifePulse lifePulse)
        {
            strategy = new HealingExplosion(data.HealthAmount);
            this.lifePulseGrenade = lifePulse;
            this.data = data;
            strategy.OnExploded += OnExploded;
            FreezeGrenade(false);
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
            lifePulseGrenade.End();
            Destroy(gameObject);
        }
    }
}