namespace OverBang.ExoWorld.Gameplay
{
    public static class CameraManagerExtensions
    {
        public static void RegisterCamera(this CameraRegister cameraRegister)
        {
            CameraManager.Instance.RegisterCamera(cameraRegister);
        }

        public static void UnregisterCamera(this CameraRegister cameraRegister)
        {
            CameraManager.Instance.UnregisterCamera(cameraRegister);
        }
    }
}