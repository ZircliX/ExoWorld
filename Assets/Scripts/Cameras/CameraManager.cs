using System.Collections.Generic;
using System.Linq;
using LTX.Singletons;
using OverBang.GameName.Metrics;
using Unity.Cinemachine;
using UnityEngine;

namespace OverBang.GameName.Cameras
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        public List<CameraRegister> Cameras { get; private set; }
        private CameraRegister currentCamera;
        
        protected override void Awake()
        {
            base.Awake();
            Cameras = new List<CameraRegister>(4);
        }
        
        public void RegisterCamera(CameraRegister camRegister)
        {
            if (Cameras.Contains(camRegister)) return;

            Cameras.Add(camRegister);
        }
        
        public void UnregisterCamera(CameraRegister camRegister)
        {
            if (!Cameras.Contains(camRegister)) return;
            
            Cameras.Remove(camRegister);
            
            if (currentCamera == camRegister.ID)
            {
                currentCamera = Cameras.Count > 0 ? Cameras[0] : null;
            }
        }
        
        public void SwitchToCamera(CameraID id)
        {
            CameraRegister newCam = Cameras.FirstOrDefault(ctx => ctx.ID == id);
            if (newCam == default)
            {
                Debug.LogError($"Cannot change to camera {id}, null or not registered.");
                return;
            }
            
            currentCamera = newCam;
            currentCamera.Cam.Priority = 100;

            foreach (CameraRegister register in Cameras)
            {
                register.Cam.Priority = 0;
            }
        }
    }
}