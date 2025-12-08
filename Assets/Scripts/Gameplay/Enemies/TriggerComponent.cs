using System;
using KBCore.Refs;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [RequireComponent(typeof(SphereCollider))]
    public class TriggerComponent : MonoBehaviour
    {
        [field: SerializeField, Self] public SphereCollider TriggerCollider { get; private set; }
        [field : SerializeField] public float TriggerRadius { get; private set; }
        
        public event Action<Transform> OnEntered;
        public event Action<Transform> OnExited;
        
        private void OnValidate()
        {
            this.ValidateRefs();
            TriggerCollider.radius = TriggerRadius;
            TriggerCollider.isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("LocalPlayer"))
            {
                OnEntered?.Invoke(other.transform);
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("LocalPlayer"))
            {
                OnExited?.Invoke(other.transform);
            }
        }
    }
}