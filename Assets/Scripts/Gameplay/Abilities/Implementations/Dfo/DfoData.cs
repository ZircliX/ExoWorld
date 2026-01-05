using Ami.BroAudio;
using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "Dfo Ability Data", menuName = "OverBang/Abilities/DfoData")]
    public class DfoData : AbilityData
    {
        [field: Header("Balise")]
        [field: SerializeField] public DfoBalise DfoBalisePrefab { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
        [field: SerializeField] public float ActivationTime { get; private set; }
        
        [field: Header("Missile Spawn")]
        [field: SerializeField] public NetworkObject MissilePrefab { get; private set; }
        [field: SerializeField] public int MissileCount { get; private set; }
        [field: SerializeField] public float DiameterSpawn { get; private set; }
        [field: SerializeField] public float HeightSpawn { get; private set; }
        
        [field: Header("Missile Data")]
        [field: SerializeField] public MissileData MissileData { get; private set; }
        
        public override IAbility CreateInstance(GameObject owner)
        {
            return new DfoAbility(this, owner);
        }
    }
}