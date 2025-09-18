using KBCore.Refs;
using OverBang.GameName.Core.Metrics;
using Unity.Cinemachine;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Cameras
{
    public class CameraRegister : MonoBehaviour
    {
        [field: SerializeField, Self] public CinemachineCamera Cam;
        [field: SerializeField] public CameraID ID;
        
        private void OnValidate() => this.ValidateRefs();
        
        private void OnEnable()
        {
            CameraManager.Instance.RegisterCamera(this);
        }

        private void OnDisable()
        {
            CameraManager.Instance.UnregisterCamera(this);
        }
    }
}