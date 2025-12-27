using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(fileName = "New DfoData", menuName = "OverBang/Abilities/DfoData")]
    public class DfoData : AbilityData
    {
        [field: SerializeField, Space] public float Range { get; private set; }
        [field: SerializeField] public int MissileCount { get; private set; }
        [field: SerializeField] public DamageInfo MissileDamage { get; private set; }
        
        public override IAbility CreateInstance(GameObject owner)
        {
            Dfo instance = new Dfo();
            instance.Initialize(this, owner);
            return instance;
        }
    }
}