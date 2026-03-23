using System;

namespace OverBang.ExoWorld.Gameplay.Cameras.Composits
{
    [System.Serializable]
    public struct CameraShakeComposite : IEquatable<CameraShakeComposite>
    {
        public float panAmplitude;
        public float tiltAmplitude;
        public float dutchAmplitude;
        public float frequency;

        public bool onlyTilt;

        public static CameraShakeComposite Default => new CameraShakeComposite
        {
            panAmplitude = 0f,
            tiltAmplitude = 0f,
            dutchAmplitude = 0f,
            frequency = 0f
        };

        public bool Equals(CameraShakeComposite other)
        {
            return panAmplitude.Equals(other.panAmplitude) && tiltAmplitude.Equals(other.tiltAmplitude) && dutchAmplitude.Equals(other.dutchAmplitude) && frequency.Equals(other.frequency);
        }

        public override bool Equals(object obj)
        {
            return obj is CameraShakeComposite other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(panAmplitude, tiltAmplitude, dutchAmplitude, frequency);
        }
    }
}