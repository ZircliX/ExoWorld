using KBCore.Refs;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class MineWax : BaseDetector
    {
        [SerializeField, Self] private Rigidbody rb;
        private IMineExplosionStrategy explosionStrategy;
        private MinesWaxData data;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void Initialize(MinesWaxData data, Vector3 direction, IMineExplosionStrategy explosionStrategy)
        {
            this.data = data;
            this.explosionStrategy = explosionStrategy;
            
            DetectionArea.GetCollider<SphereCollider>().radius = data.DetectionRadius;
            DetectionArea.SetAllowedTags("Enemy", "LocalPlayer");
            DetectionArea.SetRequireInterface<IDamageable>();
            
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);

            explosionStrategy.OnExploded += OnExploded;
        }
        
        private void OnExploded(bool endedExplosions)
        {
            // TODO : Add Sound & VFX
            
            if (endedExplosions)
                Destroy(gameObject);
        }

        protected override void OnEnter(Collider other, object target)
        {
            Detonate();
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