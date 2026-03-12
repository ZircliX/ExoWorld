using Ami.BroAudio;
using DamageNumbersPro;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [System.Serializable]
    public struct MissileData
    {
        public float Speed;
        public float LifeTime;
        public float DetonationRadius;
        public float DetonationTime;
        public DamageNumberMesh damagePrefab;
        
        [Header("Feedbacks")]
        public ParticleSystem PreviewPrefab;
        public ParticleSystem ExplosionPrefab;
        public ParticleSystem ImpactPrefab;
        public SoundID DetonationSound;
    }
}