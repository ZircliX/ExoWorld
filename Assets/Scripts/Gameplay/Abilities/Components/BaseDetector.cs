using KBCore.Refs;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public abstract class BaseDetector : MonoBehaviour
    {
        [field: SerializeField, Child] public DetectionArea DetectionArea { get; private set; }
        
        private void OnValidate() => this.ValidateRefs();

        private void OnEnable()
        {
            DetectionArea.OnEnter += OnEnter;
            DetectionArea.OnExit += OnExit;
        }

        private void OnDisable()
        {
            DetectionArea.OnEnter -= OnEnter;
            DetectionArea.OnExit -= OnExit;
        }

        protected virtual void OnEnter(Collider other, object target){}
        protected virtual void OnExit(Collider other, object target){}
    }
}