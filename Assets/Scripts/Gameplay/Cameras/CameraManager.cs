using System;
using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;
using OverBang.GameName.Core.Metrics;

namespace OverBang.GameName.Gameplay.Cameras
{
    public class CameraManager : MonoSingleton<CameraManager>
    {
        public event Action<CameraManager, CameraID> OnChangeCamera;
        public List<CameraRegister> Cameras;
        
        protected override void OnAwake()
        {
            Cameras = new List<CameraRegister>();
        }

        public void RequestCameraChange(CameraID id)
        {
            OnChangeCamera?.Invoke(this, id);
        }

        public void SwitchToCamera(CameraRegister register)
        {
            register.Cam.Priority = 100;

            if (!Cameras.Contains(register))
                Cameras.Add(register);
                
            foreach (CameraRegister reg in Cameras)
            {
                reg.Cam.Priority = 0;
            }
            
        }
    }
}