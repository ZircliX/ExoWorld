using KBCore.Refs;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Gameplay.DebugGameplay
{
    public class DebugBullet : MonoBehaviour, IPoolInstanceListener
    {
        [SerializeField, Self] private Rigidbody rb;

        private void OnValidate() => this.ValidateRefs();

        public void Fire(Transform origin)
        {
            transform.position = origin.position;
            transform.rotation = origin.rotation;
            
            rb.AddForce(origin.forward * 1000f);
        }

        public void OnSpawn(IPool pool)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector3.zero;
        }

        public void OnDespawn(IPool pool)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}