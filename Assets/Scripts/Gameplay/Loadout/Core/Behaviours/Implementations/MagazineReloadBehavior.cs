using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class MagazineReloadBehavior : IReloadBehaviour
    {
        public bool IsReloading { get; private set; }
        private Weapon weapon;
        
        public void OnInitialize(Weapon weapon)
        {
            this.weapon = weapon;
        }

        public async void OnReloadInput(InputAction.CallbackContext context)
        {
            try
            {
                if (!context.performed)
                    return;

                if (!ShouldReload())
                    return;

                await Reload();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private bool ShouldReload()
        {
            return (weapon.State.CurrentBullets < weapon.WeaponData.MagCapacity + weapon.WeaponData.UpgradeMagCap) && !IsReloading;
        }

        public async Awaitable Reload()
        {
            IsReloading = true;
            weapon.State.SetBullets(0);
            
            await Awaitable.WaitForSecondsAsync(weapon.WeaponData.ReloadTime);
            
            weapon.State.SetBullets(Mathf.RoundToInt(weapon.WeaponData.MagCapacity + weapon.WeaponData.UpgradeMagCap));
            weapon.RequestOnWeaponReloaded();
            IsReloading = false;
        }

        public void Tick(float dt) { }
    }
}