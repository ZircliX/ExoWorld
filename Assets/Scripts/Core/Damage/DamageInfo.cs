namespace OverBang.ExoWorld.Core
{
    [System.Serializable]
    public struct DamageInfo
    {
        public float baseDamage;
        public float weakSpotMultiplier;
        public float bonusDamage;
        
        public DamageInfo WithBonusDamage(float bonus)
        {
            bonusDamage = bonus;
            return this;
        }
    }
}