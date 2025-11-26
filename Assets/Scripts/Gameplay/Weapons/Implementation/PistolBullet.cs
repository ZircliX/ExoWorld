using KBCore.Refs;
using OverBang.Pooling;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PistolBullet : Bullet
    {
        [SerializeField, Self] private Rigidbody rb;
        public override IPool Pool { get; protected set; }

        private void OnValidate() => this.ValidateRefs();

        public override void Fire(Transform origin, Vector3 direction, float speed)
        {
            transform.position = origin.position;
            transform.rotation = origin.rotation;
            
            if (rb == null) return;
            rb.AddForce(direction * speed);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            Pool.Despawn(gameObject);
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