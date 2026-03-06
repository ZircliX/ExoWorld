using OverBang.ExoWorld.Core.Characters;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "OverBang/Abilities/Mines Wax", fileName = "MinesWax Ability Data")]
    public class MinesWaxData : AbilityData
    {
        [field: Space]
        [field: SerializeField] public float ThrowForce { get; private set; }
        [field: SerializeField] public NetworkObject MineWaxPrefab { get; private set; }
        [field: SerializeField] public float DetectionRadius { get; private set; }
        [field: SerializeField] public float ExplosionRadius { get; private set; }
        [field: SerializeField] public float DetonateDelay { get; private set; }
        [field: SerializeField] public ParticleSystem ExplosionVfx { get; private set; }
    }
}