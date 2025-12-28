using Ami.BroAudio;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "DfoData", menuName = "OverBang/Abilities/DfoData")]
    public class DfoData : AbilityData
    {
        [field: Header("Dfo Data")]
        [field: SerializeField] public float Cooldown { get; private set; }
        [field: SerializeField] public float Duration { get; private set; }
        
        [field: Header("Balise")]
        [field: SerializeField] public Balise BalisePrefab { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
        [field: SerializeField] public float ActivationTime { get; private set; }
        
        [field: Header("Missile Spawn")]
        [field: SerializeField] public Missile MissilePrefab { get; private set; }
        [field: SerializeField] public ParticleSystem PreviewPrefab { get; private set; }
        [field: SerializeField] public ParticleSystem ExplosionPrefab { get; private set; }
        [field: SerializeField] public ParticleSystem ImpactPrefab { get; private set; }
        [field: SerializeField] public int MissileCount { get; private set; }
        [field: SerializeField] public float DiameterSpawn { get; private set; }
        [field: SerializeField] public float HeightSpawn { get; private set; }
        [field: SerializeField] public int MissileSpeed { get; private set; }
        
        [field: Header("Missile Data")]
        [field: SerializeField] public SoundID DetonationSound { get; private set; }
        [field: SerializeField] public float DetonationTime { get; private set; }
        [field: SerializeField] public float DetonationRadius { get; private set; }
        [field: SerializeField] public DamageInfo MissileDamage { get; private set; }
        
        public override IAbility CreateInstance(GameObject owner)
        {
            return new Dfo(this, owner);
        }
    }
}