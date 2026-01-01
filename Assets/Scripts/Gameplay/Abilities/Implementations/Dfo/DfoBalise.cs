using KBCore.Refs;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DfoBalise : MonoBehaviour
    {
        [SerializeField, Self] private Rigidbody rb;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void Initialize(DfoData data, Vector3 direction)
        {
            rb.AddForce(Vector3.up * 0.5f + direction * data.ThrowForce * Time.deltaTime, ForceMode.Impulse);
            Destroy(gameObject, data.Duration);
        }
    }
}