using Ami.BroAudio;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    [System.Serializable]
    public struct MissileData
    {
        public float Speed;
        public float LifeTime;
        public float DetonationRadius;
        public float DetonationTime;
        
        [Header("Feedbacks")]
        public ParticleSystem PreviewPrefab;
        public ParticleSystem ExplosionPrefab;
        public ParticleSystem ImpactPrefab;
        public SoundID DetonationSound;
    }
}