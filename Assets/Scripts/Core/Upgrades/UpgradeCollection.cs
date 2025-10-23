using System.Collections.Generic;
using System.Linq;

namespace OverBang.GameName.Core.Upgrades
{
    public class UpgradeCollection
    {
        private List<IUpgrade> upgrades = new();

        public bool TryAdd(UpgradeData data)
        {
            if (upgrades.Contains(data)) 
                return false;
            
            upgrades.Add(data);
            return true;
        }

        public bool TryRemove(UpgradeData data)
        {
            return upgrades.Remove(data);
        }
        
        public void Clear()
        {
            upgrades.Clear();
        }
        
        public float CalculateModifiedStat(UpgradeTarget target, float baseValue)
        {
            List<IUpgrade> relevantUpgrades = upgrades.Where(u => u.Target == target).ToList();
        
            if (relevantUpgrades.Count == 0)
                return baseValue;
        
            float result = baseValue;
            foreach (UpgradeData upgrade in relevantUpgrades)
            {
                result += upgrade.GetValue(baseValue);
            }
        
            return result;
        }
    }
}