namespace OverBang.GameName.Gameplay
{
    public static class WeaponBehaviorFactory
    {
        public static IFireBehaviour CreateFireBehavior(this WeaponFireBehaviour type)
        {
            return type switch
            {
                WeaponFireBehaviour.SemiAutomatic => new SemiAutoFireBehavior(),
                WeaponFireBehaviour.FullAutomatic => new FullAutoFireBehavior(),
                WeaponFireBehaviour.Burst => new BurstFireBehavior(),
                _ => new SemiAutoFireBehavior()
            };
        }

        public static IReloadBehaviour CreateReloadBehavior(this WeaponReloadBehaviour type)
        {
            return type switch
            {
                WeaponReloadBehaviour.FullMagReload => new MagazineReloadBehavior(),
                WeaponReloadBehaviour.PerShellReload => new PerShellReloadBehavior(),
                _ => new MagazineReloadBehavior()
            };
        }
    }
}