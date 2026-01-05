using KBCore.Refs;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class MineWax : MonoBehaviour
    {
        [SerializeField, Self] private SphereCollider sphereCollider;
        [SerializeField, Self] private Rigidbody rb;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void Initialize(MinesWaxData data, Vector3 direction)
        {
            
        }
    }
}