using Ami.BroAudio;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [System.Serializable]
    public struct MissileData
    {
        public float Speed;
        public float LifeTime;
        public float DetonationRadius;
        public float DetonationTime;
        public DamageInfo Damage;
        
        [Header("Feedbacks")]
        public ParticleSystem PreviewPrefab;
        public ParticleSystem ExplosionPrefab;
        public ParticleSystem ImpactPrefab;
        public SoundID DetonationSound;
    }
}