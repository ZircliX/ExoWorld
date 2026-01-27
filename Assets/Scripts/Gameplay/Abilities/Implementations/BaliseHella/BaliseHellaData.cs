using OverBang.ExoWorld.Core.Characters;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [CreateAssetMenu(menuName = "OverBang/Abilities/Balise Hella", fileName = "BaliseHella Ability Data")]
    public class BaliseHellaData : AbilityData
    {
        [field: Space]
        [field: SerializeField] public BaliseHella BalisePrefab { get; private set; }
        [field: SerializeField] public ParticleSystem HealingCircle { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
    }
}