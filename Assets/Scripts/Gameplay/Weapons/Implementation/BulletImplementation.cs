using KBCore.Refs;
using OverBang.GameName.Core;
using OverBang.Pooling;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class BulletImplementation : Bullet
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private SphereCollider sc;
        public override IPool Pool { get; protected set; }

        private BulletData data;
        
        private Vector3 previousPosition;
        private RaycastHit[] results;

        private void OnValidate() => this.ValidateRefs();

        public override void Fire(Transform origin, Vector3 direction, BulletData bulletData)
        {
            transform.position = origin.position;
            transform.rotation = origin.rotation;

            data = bulletData;
            
            if (rb == null) return;
            rb.AddForce(direction * data.BulletSpeed);
        }

        private void FixedUpdate()
        {
            Vector3 currentPosition = transform.position;
            Vector3 distance = currentPosition - previousPosition;
            Vector3 direction = distance.normalized;
            LayerMask mask = GameMetrics.Global.HittableLayers;
            
            int hitSize = Physics.SphereCastNonAlloc(currentPosition, sc.radius, direction, results, distance.sqrMagnitude, mask, QueryTriggerInteraction.Collide);

            for (int i = 0; i < hitSize; i++)
            {
                if (i <= data.Penetration - 1)
                {
                    RaycastHit hit = results[i];

                        if (hit.collider.TryGetComponent(out IDamageable damageable))
                        {
                            Debug.Log($"Bullet hit : {hit.collider.gameObject.name}", hit.collider.gameObject);
                            damageable.TakeDamage(data.Damage);
                        }
                    }
                    else
                    {
                        damageable.TakeDamage(data.BulletDamage);
                    }
                }
                else
                {
                    OnDespawn(Pool);
                }
            }
            
            previousPosition = transform.position;
        }

        public override void OnSpawn(IPool pool)
        {
            Pool = pool;

            if (rb == null) return;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        public override void OnDespawn(IPool pool)
        {
            if (rb == null) return;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}