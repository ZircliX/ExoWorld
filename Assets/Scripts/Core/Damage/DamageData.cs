using OverBang.ExoWorld.Core.Inventory;

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

        public readonly RuntimeDamageData GetRuntimeDamage()
        {
            return new RuntimeDamageData
            {
                finalDamage = baseDamage + bonusDamage,
                weakSpotMultiplier = weakSpotMultiplier
            };
        }
    }

    public struct RuntimeDamageData
    {
        public object source;
        public float finalDamage;
        public float weakSpotMultiplier;
        public DamageType damageType;
        public ScriptableItemData itemData;
    }
    
    public enum DamageType { Physical, Explosive, Projectile }
}