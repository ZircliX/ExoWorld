using KBCore.Refs;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class MineWax : BaseDetector
    {
        [SerializeField, Self] private Rigidbody rb;
        private IExplosionStrategy explosionStrategy;
        private MinesWaxData data;

        private bool canDetonate;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void Initialize(MinesWaxData data, Vector3 direction, IExplosionStrategy explosionStrategy)
        {
            this.data = data;
            this.explosionStrategy = explosionStrategy;
            
            DetectionArea.GetCollider<SphereCollider>().radius = data.DetectionRadius;
            DetectionArea.SetAllowedTags("Enemy", "LocalPlayer");
            DetectionArea.SetRequireInterface<IDamageable>();
            
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);

            explosionStrategy.OnExploded += OnExploded;
            Invoke(nameof(SetDetonate), data.DetonateDelay);
        }

        private void SetDetonate()
        {
            canDetonate = true;
        }
        
        private void OnExploded(bool endedExplosions)
        {
            // TODO : Add Sound & VFX
            ParticleSystem ps = Instantiate(data.ExplosionVfx, transform.position, Quaternion.identity);
            Destroy(ps.gameObject, ps.main.duration);

            if (endedExplosions)
            {
                explosionStrategy.OnExploded -= OnExploded;
                Destroy(gameObject);
            }
        }

        protected override void OnEnter(Collider other, object target)
        {
            if (!canDetonate) return;
            
            Detonate();
            canDetonate = false;
        }

        private void Detonate()
        {
            explosionStrategy.Explode(() =>
            {
                Collider[] colliders = Physics.OverlapSphere(
                    transform.position,
                    data.ExplosionRadius,
                    GameMetrics.Global.HittableLayers,
                    QueryTriggerInteraction.Collide);

                return colliders;
            });
        }
    }
}