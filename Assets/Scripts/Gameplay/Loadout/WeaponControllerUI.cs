using DG.Tweening;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Inventory;
using OverBang.ExoWorld.Gameplay.Upgrade;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class WeaponControllerUI : MonoBehaviour
    {
        [SerializeField] private WeaponController controller;
        
        [Header("HUD")]
        
        [SerializeField] private Image weaponIcon;
        [SerializeField] private TMP_Text ammoText;
        [SerializeField] private TMP_Text totalAmmoText;
        [SerializeField] private Image weaponPoint1;
        [SerializeField] private Image weaponPoint2;
        private bool IsFirstSelected = true;
        
        private Weapon currentWeapon;
        private LocalGamePlayer player;

        private void Awake()
        {
            player = GamePlayerManager.Instance.GetLocalPlayer();
        }

        private void OnEnable()
        {
            controller.OnWeaponChanged += OnWeaponChanged;
            controller.LoadoutController.OnActiveStateChanged += OnActiveStateChanged;
            UpgradeManager.Instance.OnUpgrade += HandleUpgrade;
            player.Inventory.OnItemQuantityChanged += OnItemQuantityChanged;

            // If a weapon is already equipped when HUD appears
            if (controller.CurrentWeapon != null)
                OnWeaponChanged(null, controller.CurrentWeapon);
        }

        private void OnDisable()
        {
            controller.OnWeaponChanged -= OnWeaponChanged;
            controller.LoadoutController.OnActiveStateChanged -= OnActiveStateChanged;
            UpgradeManager.Instance.OnUpgrade -= HandleUpgrade;
            player.Inventory.OnItemQuantityChanged -= OnItemQuantityChanged;
            UnsubscribeCurrentWeapon();
        }

        private void OnItemQuantityChanged(ItemData item)
        {
            if (item.ItemId == currentWeapon.WeaponData.BulletData.ItemData.Data.ItemId)
            {
                totalAmmoText.text = (item.Quantity / currentWeapon.WeaponData.BulletsPerShot).ToString();
            }
        }

        private void OnWeaponChanged(Weapon previous, Weapon current)
        {
            Debug.Log("OnWeaponChange Event");
            UnsubscribeCurrentWeapon();
            if (previous != null) SwitchWeaponPoint();
                
            currentWeapon = controller.CurrentWeapon;

            if (currentWeapon != null)
            {
                currentWeapon.OnWeaponFired += OnWeaponFired;
                currentWeapon.OnWeaponReload += OnWeaponReload;
                currentWeapon.OnWeaponSetCurrent += OnWeaponSetCurrent;
            }

            UpdateWeaponUI();
        }

        private void SwitchWeaponPoint()
        {
            IsFirstSelected = !IsFirstSelected;
            weaponPoint1.DOKill();
            weaponPoint2.DOKill();
            weaponPoint1.DOFade(IsFirstSelected ? 1 : 0.5f, 0.2f);
            weaponPoint2.DOFade(IsFirstSelected ? 0.5f : 1f, 0.2f);
        }

        private void UnsubscribeCurrentWeapon()
        {
            if (currentWeapon == null)
                return;

            currentWeapon.OnWeaponFired -= OnWeaponFired;
            currentWeapon.OnWeaponReload -= OnWeaponReload;
            currentWeapon.OnWeaponSetCurrent -= OnWeaponSetCurrent;
            currentWeapon = null;
        }
        
        private void OnWeaponFired()
        {
            UpdateWeaponUI();
        }
        
        private void OnWeaponReload(bool isReloading)
        {
            if (isReloading)
            {
                OnActiveStateChanged(true);
            }
            else
            {
                OnActiveStateChanged(false);
                UpdateWeaponUI();
            }
        }
        
        private void OnWeaponSetCurrent(bool isCurrent)
        {
            OnActiveStateChanged(false); // Fade to 1 in case of reloading and switching
        }
        
        private void HandleUpgrade()
        {
            WeaponData data = currentWeapon.WeaponData;
            currentWeapon.State.SetBullets((data.MagCapacity + data.UpgradeMagCap) / data.BulletsPerShot);
            UpdateWeaponUI();
        }

        private void OnActiveStateChanged(bool onlyUI)
        {
            weaponIcon.DOKill();
            ammoText.DOKill();
            ammoText.color = Color.white;
            totalAmmoText.DOKill();
            
            weaponIcon.DOFade(!onlyUI ? 1 : 0.25f, 0.3f);
            ammoText.DOFade(!onlyUI ? 1 : 0.25f, 0.3f);
            totalAmmoText.DOFade(!onlyUI ? 1 : 0.25f, 0.3f);
        }

        private void UpdateWeaponUI()
        {
            if (currentWeapon == null)
            {
                ClearUI();
                return;
            }

            WeaponData data = currentWeapon.WeaponData;
            int bulletsPerShot = data.BulletsPerShot;

            if (weaponIcon != null && data.WeaponSprite != null)
                weaponIcon.sprite = data.WeaponSprite;

            int currentAmmo = currentWeapon.State.CurrentBullets / bulletsPerShot;

            if (ammoText != null)
            {
                if (currentAmmo == 0)
                {
                    ammoText.DOColor(Color.red, 0.2f).OnComplete(() =>
                    {
                        ammoText.DOColor(Color.white, 1.5f);
                    });
                }
                ammoText.text = $"{currentAmmo}";
                totalAmmoText.text = (player.Inventory.GetItemQuantity(data.BulletData.ItemData.Data.ItemId) / bulletsPerShot).ToString();
            }
        }

        private void ClearUI()
        {
            if (weaponIcon != null)
                weaponIcon.sprite = null;

            if (ammoText != null)
                ammoText.text = string.Empty;
        }
    }
}