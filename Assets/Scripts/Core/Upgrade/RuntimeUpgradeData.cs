namespace OverBang.ExoWorld.Core
{
    public struct RuntimeUpgradeData
    {
        public UpgradeData upgradeData;
        public int level;
        public float bonus;
        public float finalBonus;
        public int cost;
        public int initialCost;


        public void Initialize()
        {
            bonus = upgradeData.Bonus;
            cost = upgradeData.Cost;
            initialCost = cost;
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