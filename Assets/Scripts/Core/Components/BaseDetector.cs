using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Components
{
    public abstract class BaseDetector : MonoBehaviour
    {
        [field: SerializeField] public DetectionArea DetectionArea { get; private set; }
        
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
    
    public abstract class NetworkBaseDetector : NetworkBehaviour
    {
        [field: SerializeField] public DetectionArea DetectionArea { get; private set; }

        private void OnEnable()
        {
            if (!IsOwner)
            {
                Destroy(DetectionArea);
                return;
            }
            
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