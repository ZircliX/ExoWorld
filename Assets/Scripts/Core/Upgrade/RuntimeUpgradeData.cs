namespace OverBang.GameName.Core
{
    public struct RuntimeUpgradeData
    {
        public UpgradeData upgradeData;
        public int level;
        public float bonus;
        public float finalBonus;
        public int cost;


        public void Initialize()
        {
            bonus = upgradeData.Bonus;
            cost = upgradeData.Cost;
        }
        
        
        public RuntimeUpgradeData SetValue(float vBonus, int vLevel, int vCost)
        {
            finalBonus = vBonus;
            level = vLevel;
            cost = vCost;
            return this;
        }
        
    }
}