using System;
using System.Threading;
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
        
        private CancellationTokenSource reloadCts;
        
        public void OnInitialize(Weapon weapon)
        {
            this.weapon = weapon;
            weapon.OnWeaponSetCurrent += OnWeaponSetCurrent;
            player = GamePlayerManager.Instance.GetLocalPlayer();
        }

        private void OnWeaponSetCurrent(bool val)
        {
            if (!val) CancelReload();
        }

        private void CancelReload()
        {
            reloadCts?.Cancel();
            reloadCts?.Dispose();
            reloadCts = null;
            IsReloading = false;
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
            
            CancelReload(); // cancel any ongoing reload first
            reloadCts = new CancellationTokenSource();
            IsReloading = true;
            
            PlayerCDController.Instance.FireDialogue(weapon.WeaponData.ReloadDialogueData);
            weapon.State.SetBullets(0);
            weapon.RequestOnWeaponReloaded(true);
            
            try
            {
                await Awaitable.WaitForSecondsAsync(weapon.WeaponData.ReloadTime, reloadCts.Token);
            }
            catch (OperationCanceledException)
            {
                weapon.State.SetBullets(current); // restore bullets on cancel
                return;
            }

            int quantityRequested = (weapon.WeaponData.MagCapacity + weapon.WeaponData.UpgradeMagCap) - current;
            int quantityReceived = player.Inventory.RemoveItem(weapon.WeaponData.BulletData.ItemData.Data.ItemId, quantityRequested);
            
            weapon.State.SetBullets(current + quantityReceived);
            weapon.RequestOnWeaponReloaded(false);
            IsReloading = false;
        }

        public void Tick(float dt) { }
    }
}