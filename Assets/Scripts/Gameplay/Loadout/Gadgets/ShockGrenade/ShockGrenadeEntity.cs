using OverBang.ExoWorld.Core;
using OverBang.ExoWorld.Gameplay.Abilities;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public class ShockGrenadeEntity : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        
        private IExplosionStrategy strategy;

        private ShockGrenadeData data;
        private float time;
        private bool isDetonated;
        
        public void Initialize(ShockGrenadeData data, Vector3 direction)
        {
            strategy = new StandardExplosion(data.DamageInfo);
            this.data = data;
            strategy.OnExploded += OnExploded;
            
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);
            
        }

        private void OnExploded(bool terminated)
        {
            if (terminated)
            {
                strategy.OnExploded -= OnExploded;
                End();
            }
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

        private void End()
        {
            Destroy(gameObject);
            
        }
    }
}