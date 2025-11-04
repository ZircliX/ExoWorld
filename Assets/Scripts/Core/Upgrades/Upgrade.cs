using UnityEngine;

namespace OverBang.GameName.Core.Upgrades
{
    public abstract class Upgrade : ScriptableObject
    {
        [field: SerializeField] public string UpgradeName { get; private set; }
        [field: SerializeField] public string UpgradeDescription { get; private set; }
        [field: SerializeField] public Sprite UpgradeSprite { get; private set; }
        [field: SerializeField] public ResourceAmount UpgradeCost { get; private set; }
        
        public abstract void ApplyUpgrade();
    }
}