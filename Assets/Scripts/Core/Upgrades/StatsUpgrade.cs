using UnityEngine;

namespace OverBang.GameName.Core.Upgrades
{
    [CreateAssetMenu(menuName = "OverBang/Upgrades/Stats Upgrade", fileName = "New Stats Upgrade", order = 0)]
    public class StatsUpgrade : Upgrade
    {
        [System.Serializable]
        public struct UpgradeContext
        {
            public UpgradeTarget Target;
            public float Value;
        }
        
        [field: SerializeField] public UpgradeContext[] UpgradesToApply { get; private set; }
        
        public override void ApplyUpgrade()
        {
            for (int index = 0; index < UpgradesToApply.Length; index++)
            {
                UpgradeContext ctx = UpgradesToApply[index];
                Debug.Log($"Applying : {ctx.Target} with {ctx.Value} points");
            }
        }
    }
}