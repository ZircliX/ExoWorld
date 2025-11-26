using System;
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

                if (weapon.State.CurrentBullets == weapon.State.MaxBullets)
                    return;

                weapon.State.SetBullets(0, false);
                await Awaitable.WaitForSecondsAsync(weapon.WeaponData.ReloadTime);
                weapon.State.SetBullets(weapon.State.MaxBullets, true);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Tick(float dt) { }
    }
}