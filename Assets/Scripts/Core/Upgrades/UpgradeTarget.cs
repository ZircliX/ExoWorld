namespace OverBang.GameName.Core.Upgrades
{
    [System.Serializable]
    public enum UpgradeTarget
    {
        // Player Stats
        PlayerHealth,
        PlayerResistance,
        PlayerStrength,
        PlayerHeal,
        PlayerProductivity,
    
        // Weapon Stats
        WeaponDamage,
        WeaponFireRate,
        WeaponReloadSpeed,
        WeaponMagazineSize,
    
        // Bullet Stats
        BulletDamage,
        BulletVelocity,
        BulletPenetration,
    
        // Meta
        Custom
    }
}