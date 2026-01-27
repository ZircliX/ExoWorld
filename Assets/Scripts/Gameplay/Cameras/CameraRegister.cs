using KBCore.Refs;
using OverBang.ExoWorld.Core.Metrics;
using Unity.Cinemachine;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Cameras
{
    public class CameraRegister : MonoBehaviour
    {
        [field: SerializeField, Self] public CinemachineCamera Cam;
        [field: SerializeField] public CameraID ID;
        
        private void OnValidate() => this.ValidateRefs();
        
        private void OnEnable()
        {
            this.RegisterCamera();
        }

        private void OnDisable()
        {
            this.UnregisterCamera();
        }
    }
}