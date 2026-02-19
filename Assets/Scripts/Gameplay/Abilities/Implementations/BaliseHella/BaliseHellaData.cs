using OverBang.ExoWorld.Core.Characters;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "OverBang/Abilities/Balise Hella", fileName = "BaliseHella Ability Data")]
    public class BaliseHellaData : AbilityData
    {
        [field: Space]
        [field: SerializeField] public NetworkObject BalisePrefab { get; private set; }
        [field: SerializeField] public BaliseVfxInitializer VfxInitializer { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
    }
}