using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Abilities;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout.BurstGadget
{
    public class BurstGrenadeEntity : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        
        private IExplosionStrategy strategy;

        private BurstGrenadeData data;
        private float time;
        private bool isDetonated;

        public void FreezeGrenade(bool value)
        {
            rb.isKinematic = value;
        }
        
        public void Initialize(BurstGrenadeData data, Vector3 direction)
        {
            strategy = new StandardExplosion(data.DamageData);
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
            if (terminated)
            {
                strategy.OnExploded -= OnExploded;
                End();
            }
        }
        
        private void End()
        {
            Destroy(gameObject);
            
        }
    }
}