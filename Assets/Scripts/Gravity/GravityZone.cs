using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;

namespace DeadLink.Gravity
{
    [RequireComponent(typeof(Collider))]
    public abstract class GravityZone : MonoBehaviour
    {
        [SerializeField] private float gravityStrength = 1f;
        [SerializeField, Range(10, 50)] private int priority;
        private List<GravityReceiver> gravityReceivers;
        
        [SerializeField, Self] protected Collider myCollider;

        protected virtual void OnValidate()
        {
            this.ValidateRefs();
        }

        protected virtual void OnEnable()
        {
            GravityManager.Instance.RegisterGravityZone(this);
        }

        protected virtual void OnDisable()
        {
            if (!GravityManager.HasInstance) return;
            GravityManager.Instance.UnregisterGravityZone(this);
        }

        protected void Awake()
        {
            gravityReceivers = new List<GravityReceiver>();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out GravityReceiver gravityReceiver))
            {
                //Debug.Log($"Entered {other.name} into {name}");
                gravityReceivers.Add(gravityReceiver);
                gravityReceiver.Gravity.AddPriority(this, priority);
            }
        }
        
        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out GravityReceiver gravityReceiver))
            {
                //Debug.Log($"Exited {other.name} into {name}");
                gravityReceivers.Remove(gravityReceiver);
                gravityReceiver.Gravity.RemovePriority(this);
            }
        }
        
        internal virtual void OnFixedUpdate()
        {
            foreach (GravityReceiver receiver in gravityReceivers)
            {
                Vector3 gravityForReceiver = GetGravityForReceiver(receiver).normalized;
                receiver.Gravity.Write(this, gravityForReceiver * gravityStrength);
            }
        }
        
        protected abstract Vector3 GetGravityForReceiver(GravityReceiver receiver);
    }
}