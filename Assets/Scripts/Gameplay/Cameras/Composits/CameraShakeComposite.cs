namespace OverBang.GameName.Gameplay.Cameras
{
    [System.Serializable]
    public struct CameraShakeComposite
    {
        public float Amplitude;
        public float Frequency;
        public float Duration;

        public CameraShakeComposite(float amplitude, float frequency, float duration)
        {
            Amplitude = amplitude;
            Frequency = frequency;
            Duration = duration;
        }
        
        public override string ToString()
        {
            return $"Amplitude: {Amplitude}, Frequency: {Frequency}, Duration: {Duration}";
        }
    }
}