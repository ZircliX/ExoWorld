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
        
        private Weapon currentWeapon;
        private LocalGamePlayer player;

        private void Awake()
        {
            player = GamePlayerManager.Instance.GetLocalPlayer();
        }

        private void OnEnable()
        {
            controller.OnWeaponChanged += OnWeaponChanged;
            controller.OnActiveStateChanged += OnActiveStateChanged;
            UpgradeManager.Instance.OnUpgrade += HandleUpgrade;
            player.Inventory.OnItemQuantityChanged += OnItemQuantityChanged;

            // If a weapon is already equipped when HUD appears
            if (controller.CurrentWeapon != null)
                OnWeaponChanged(null, controller.CurrentWeapon);
        }

        private void OnDisable()
        {
            controller.OnWeaponChanged -= OnWeaponChanged;
            controller.OnActiveStateChanged -= OnActiveStateChanged;
            UpgradeManager.Instance.OnUpgrade -= HandleUpgrade;
            player.Inventory.OnItemQuantityChanged -= OnItemQuantityChanged;
            UnsubscribeCurrentWeapon();
        }

        private void OnItemQuantityChanged(ItemData item)
        {
            if (item.ItemId == currentWeapon.WeaponData.BulletData.ItemData.Data.ItemId)
            {
                totalAmmoText.text = item.Quantity.ToString();
            }
        }

        private void OnWeaponChanged(Weapon previous, Weapon current)
        {
            UnsubscribeCurrentWeapon();

            currentWeapon = controller.CurrentWeapon;

            if (currentWeapon != null)
            {
                currentWeapon.OnWeaponFired += OnWeaponEvent;
                currentWeapon.OnWeaponReloaded += OnWeaponEvent;
            }

            UpdateWeaponUI();
        }

        private void UnsubscribeCurrentWeapon()
        {
            if (currentWeapon == null)
                return;

            currentWeapon.OnWeaponFired -= OnWeaponEvent;
            currentWeapon.OnWeaponReloaded -= OnWeaponEvent;
            currentWeapon = null;
        }

        private void OnWeaponEvent()
        {
            UpdateWeaponUI();
        }
        
        private void HandleUpgrade()
        {
            WeaponData data = currentWeapon.WeaponData;
            currentWeapon.State.SetBullets((data.MagCapacity + data.UpgradeMagCap) / data.BulletsPerShot);
            UpdateWeaponUI();
        }

        private void OnActiveStateChanged(bool active)
        {
            weaponIcon.DOFade(active ? 1 : 0.25f, 0.3f);
            ammoText.DOFade(active ? 1 : 0.25f, 0);
            totalAmmoText.DOFade(active ? 1 : 0.25f, 0);
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