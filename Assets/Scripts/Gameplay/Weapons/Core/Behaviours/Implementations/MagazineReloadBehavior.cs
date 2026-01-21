using System;
using OverBang.GameName.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class MagazineReloadBehavior : IReloadBehaviour
    {
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

                if (weapon.State.CurrentBullets >= weapon.WeaponData.MagCapacity + weapon.WeaponData.UpgradeMagCap)
                    return;

                await Reload();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public async Awaitable Reload()
        {
            weapon.State.SetBullets(0, true);
            await Awaitable.WaitForSecondsAsync(weapon.WeaponData.ReloadTime);
            weapon.State.SetBullets(Mathf.RoundToInt(weapon.WeaponData.MagCapacity + weapon.WeaponData.UpgradeMagCap), false);
            weapon.RequestOnWeaponReloaded();
        }

        public void Tick(float dt) { }
    }
}