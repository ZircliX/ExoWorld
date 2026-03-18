using System;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Gameplay.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class MagazineReloadBehavior : IReloadBehaviour
    {
        public bool IsReloading { get; private set; }
        private Weapon weapon;
        private LocalGamePlayer player;
        
        public void OnInitialize(Weapon weapon)
        {
            this.weapon = weapon;
            player = GamePlayerManager.Instance.GetLocalPlayer();
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
            if (player.Inventory.GetItemQuantity(weapon.WeaponData.BulletData.ItemData.Data.ItemId) <= 0)
                return;

            int current = weapon.State.CurrentBullets;
            
            IsReloading = true;
            
            PlayerCDController.Instance.FireDialogue(weapon.WeaponData.ReloadGialoguesData);
            
            weapon.State.SetBullets(0);
            
            await Awaitable.WaitForSecondsAsync(weapon.WeaponData.ReloadTime);

            int quantityRequested = (weapon.WeaponData.MagCapacity + weapon.WeaponData.UpgradeMagCap) - current;
            int quantityReceived = player.Inventory.RemoveItem(weapon.WeaponData.BulletData.ItemData.Data.ItemId, quantityRequested);
            
            weapon.State.SetBullets(current + quantityReceived);
            weapon.RequestOnWeaponReloaded();
            IsReloading = false;
        }

        public void Tick(float dt) { }
    }
}