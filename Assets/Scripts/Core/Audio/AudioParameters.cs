using UnityEngine;

namespace OverBang.GameName.Core.Audio
{
    public struct AudioParameters
    {
        public AudioClip AudioClip;
        public Vector3 Position;
        public float Volume;
        public float Pitch;
        public float SpatialBlend;
        public bool Loop;
    }
}