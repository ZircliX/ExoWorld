using KBCore.Refs;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    public class EnemyAnimator : MonoBehaviour
    {
        [field : SerializeField] public Animator ModelAnimator { get; private set; }
        [SerializeField, Child] private Rigidbody[] ragdollRigidbodies;
        [SerializeField, Child] private Collider[] ragdollColliders;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void Ragdoll(bool value)
        {
            if (ragdollRigidbodies != null && ragdollColliders != null)
            {
                ModelAnimator.enabled = !value;

                foreach (Rigidbody rb in ragdollRigidbodies)
                {
                    rb.isKinematic = !value;
                }

                foreach (Collider col in ragdollColliders)
                {
                    col.enabled = value;
                }
            }
        }

        public void SetBool(string root, bool value)
        {
            ModelAnimator.SetBool(root, value);
        }
        
    }
}