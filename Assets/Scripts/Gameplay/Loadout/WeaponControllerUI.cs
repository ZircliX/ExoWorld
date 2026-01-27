using DG.Tweening;
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
        
        private Weapon currentWeapon;
        
        private void OnEnable()
        {
            controller.OnWeaponChanged += OnWeaponChanged;
            UpgradeManager.Instance.OnUpgrade += HandleUpgrade;

            // If a weapon is already equipped when HUD appears
            if (controller.CurrentWeapon != null)
                OnWeaponChanged();
        }

        private void OnDisable()
        {
            controller.OnWeaponChanged -= OnWeaponChanged;
            UpgradeManager.Instance.OnUpgrade -= HandleUpgrade;
            UnsubscribeCurrentWeapon();
        }

        private void OnWeaponChanged()
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
            currentWeapon.State.SetBullets((data.MagCapacity + data.UpgradeMagCap) / data.BulletsPerShot, false);
            UpdateWeaponUI();
        }

        private void UpdateWeaponUI()
        {
            if (currentWeapon == null)
            {
                ClearUI();
                return;
            }

            WeaponData data = currentWeapon.WeaponData;

            if (weaponIcon != null && data.WeaponSprite != null)
                weaponIcon.sprite = data.WeaponSprite;

            int currentAmmo = currentWeapon.State.CurrentBullets / currentWeapon.WeaponData.BulletsPerShot;
            int magSize = (data.MagCapacity + data.UpgradeMagCap) / data.BulletsPerShot;

            if (ammoText != null)
            {
                if (currentAmmo == 0)
                {
                    ammoText.DOColor(Color.red, 0.2f).OnComplete(() =>
                    {
                        ammoText.DOColor(Color.white, 1.5f);
                    });
                }
                ammoText.text = $"{currentAmmo}/{magSize}";
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