using System;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public static class PlayerLoadout
    {
        public static Loadout Loadout { get; private set; }
        public static event Action OnLoadoutChanged;

        public static void SetWeapons(WeaponData primary, WeaponData secondary)
        {
            Loadout loadout = Loadout;
            loadout.primaryWeapon = primary;
            loadout.secondaryWeapon = secondary;
            
            Loadout = loadout;
            OnLoadoutChanged?.Invoke();
        }
    }
}