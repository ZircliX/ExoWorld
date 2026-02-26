using OverBang.ExoWorld.Core.Characters;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [CreateAssetMenu(fileName = "AppareilZeta Ability Data", menuName = "OverBang/Abilities/AppareilZeta")]
    public class AppareilZetaData : AbilityData
    {
        [field: Space]
        [field: SerializeField] public NetworkObject Prefab { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
    }
}