using KBCore.Refs;
using UnityEngine;

namespace DeadLink.Gravity
{
    [RequireComponent(typeof(Rigidbody))]
    public class RigidbodyGravityReceiver : GravityReceiver
    {
        [SerializeField, Self] private Rigidbody rb;

        public override Vector3 Position => rb.position;
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void FixedUpdate()
        {
            rb.AddForce(Gravity, ForceMode.Force);
        }
    }
}