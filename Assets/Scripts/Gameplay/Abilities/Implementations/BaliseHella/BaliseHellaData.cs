using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/Abilities/Balise Hella", fileName = "BaliseHella Ability Data")]
    public class BaliseHellaData : AbilityData
    {
        [field: SerializeField] public BaliseHella BalisePrefab { get; private set; }
        [field: SerializeField] public ParticleSystem HealingCircle { get; private set; }
        [field: SerializeField] public float ThrowForce { get; private set; }
        [field: SerializeField] public float Radius { get; private set; }
        [field: SerializeField] public float MinHp { get; private set; }
        
        public override IAbility CreateInstance(GameObject owner)
        {
            return new BaliseHellaAbility(this, owner);
        }
    }
}