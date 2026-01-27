using System;
using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;
using OverBang.ExoWorld.Core.Metrics;

namespace OverBang.ExoWorld.Gameplay.Cameras
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        public event Action<CameraRegister> OnChangeCamera;
        public HashSet<CameraRegister> Cameras;
        
        protected override void OnAwake()
        {
            Cameras = new HashSet<CameraRegister>();
        }

        public void RegisterCamera(CameraRegister cameraRegister)
        {
            Cameras.Add(cameraRegister);
        }
        
        public void UnregisterCamera(CameraRegister cameraRegister)
        {
            Cameras.Remove(cameraRegister);
        }

        public void RequestCameraChange(CameraID id)
        {
            foreach (CameraRegister reg in Cameras)
            {
                reg.Cam.Priority = 0;
                if (reg.ID == id)
                {
                    reg.Cam.Priority = 100;
                    OnChangeCamera?.Invoke(reg);
                }
            }
        }
    }
}