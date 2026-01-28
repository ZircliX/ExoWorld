namespace OverBang.ExoWorld.Core.Damage
{
    [System.Serializable]
    public struct DamageData
    {
        public float baseDamage;
        public float weakSpotMultiplier;
        private float bonusDamage;
        
        public DamageData WithBonusDamage(float bonus)
        {
            bonusDamage = bonus;
            return this;
        }
    }

    public struct RuntimeDamageData
    {
        
    }
}