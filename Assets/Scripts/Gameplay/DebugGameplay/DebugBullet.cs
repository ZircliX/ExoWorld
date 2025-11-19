using KBCore.Refs;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DebugBullet : MonoBehaviour, IPoolInstanceListener
    {
        [SerializeField, Self] private Rigidbody rb;
        public IPool Pool { get; private set; }

        private void OnValidate() => this.ValidateRefs();

        public void Fire(Transform origin)
        {
            transform.position = origin.position;
            transform.rotation = origin.rotation;
            
            if (rb == null) return;
            rb.AddForce(origin.forward * 1000f);
        }

        public void OnSpawn(IPool pool)
        {
            Pool = pool;

            if (rb == null) return;
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        public void OnDespawn(IPool pool)
        {
            if (rb == null) return;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                Pool.Despawn(gameObject);
        }
    }
}