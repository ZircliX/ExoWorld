namespace OverBang.GameName.Cameras
{
    [System.Serializable]
    public struct CameraEffectComposite
    {
        public float Dutch;
        public float FovScale;
        public float Speed;
        
        public CameraEffectComposite(float dutch, float fovScale, float lerpSpeed)
        {
            Dutch = dutch;
            FovScale = fovScale;
            Speed = lerpSpeed;
        }
        
        public override string ToString()
        {
            return $"Dutch: {Dutch}, FOV: {FovScale}, LerpSpeed: {Speed}";
        }

        public static CameraEffectComposite Default => new CameraEffectComposite(0, 1, 0.35f);
    }
}