using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [DisallowMultipleComponent]
    public class DetectionArea : MonoBehaviour
    {
        [field: SerializeField, Required] public Collider DetectionCollider { get; private set; }
        
        private readonly HashSet<string> allowedTags = new HashSet<string>();
        private LayerMask allowedLayers;
        private Type requiredInterface;
        
        public event Action<Collider, object> OnEnter;
        public event Action<Collider, object> OnExit;
        
        private void OnValidate()
        {
            DetectionCollider.isTrigger = true;
        }
        
        public T GetCollider<T>() where T : Collider
        {
            return DetectionCollider as T;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!IsValidTarget(other))
                return;

            if (!TryGetRequiredInterface(other, out object target))
                return;
            
            OnEnter?.Invoke(other, target);
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsValidTarget(other))
                return;

            if (!TryGetRequiredInterface(other, out object target))
                return;
            
            OnExit?.Invoke(other, target);
        }
        
        private bool IsValidTarget(Collider other)
        {
            if (!MatchesInterface(other))
                return false;

            if (!IsInAllowedTags(other.gameObject))
                return false;
            
            if (!IsInLayerMask(other.gameObject.layer))
                return false;
            
            return true;
        }

        #region Checks
        
        private bool TryGetRequiredInterface(Collider other, out object target)
        {
            target = null;

            if (requiredInterface == null)
                return true;

            var component = other.GetComponent(requiredInterface);
            if (component == null)
                return false;

            target = component;
            return true;
        }
        
        private bool MatchesInterface(Collider other)
        {
            if (requiredInterface == null)
                return true;

            return other.GetComponent(requiredInterface) != null;
        }
        
        private bool IsInLayerMask(int layer)
        {
            if (allowedLayers == 0) return true;
            return (allowedLayers & (1 << layer)) != 0;
        }

        private bool IsInAllowedTags(GameObject go)
        {
            return allowedTags.Count == 0 || allowedTags.Contains(go.tag);
        }
        
        #endregion

        #region Public API

        public void SetRequireInterface<T>() where T : class
        {
            requiredInterface = typeof(T);
        }
        
        public void SetAllowedLayerMask(LayerMask mask)
        {
            allowedLayers = mask;
        }
        
        public void SetAllowedTags(params string[] tags)
        {
            allowedTags.Clear();
            allowedTags.AddRange(tags);
        }

        #endregion
    }
}