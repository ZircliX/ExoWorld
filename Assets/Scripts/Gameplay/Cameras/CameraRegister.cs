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
            CameraManager.Instance.OnChangeCamera += OnChangeCamera;
        }

        private void OnDisable()
        {
            CameraManager.Instance.OnChangeCamera -= OnChangeCamera;
        }

        private void OnChangeCamera(CameraManager cameraManager, CameraID cameraID)
        {
            if (ID != cameraID)
                return;
            
            cameraManager.SwitchToCamera(this);
        }
    }
}