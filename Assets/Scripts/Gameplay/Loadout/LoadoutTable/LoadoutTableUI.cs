using System.Collections.Generic;
using System.Globalization;
using OverBang.ExoWorld.Core.Menus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class LoadoutTableUI : NavigablePanel
    {
        [SerializeField] private Button selectButton;
        [SerializeField] private Button deselectButton;
        [SerializeField] private LoadoutTable loadout;
        [SerializeField] private List<WeaponHolderUI> weaponHolderUis;
        
        [SerializeField, Space] private Image weaponIcon;
        [SerializeField] private TMP_Text weaponName;
        [SerializeField] private TMP_Text weaponType;
        [SerializeField] private TMP_Text weaponDescription;
        
        [SerializeField, Space] private TMP_Text weaponDamage;
        [SerializeField] private TMP_Text weaponShootRate;
        [SerializeField] private TMP_Text weaponMagCapacity;

        [field: SerializeField, Space] public Transform SelectedWeaponTarget { get; private set; }
        [SerializeField] private CanvasPanelGroup weaponHolderPanelGroup;
        [SerializeField] private CanvasPanelGroup bottomPanelGroup;

        public WeaponHolderUI CurrentWeaponHolderUI { get; private set;}
        
        protected override void Awake()
        {
            base.Awake();
            foreach (WeaponHolderUI weaponHolderUi in weaponHolderUis)
            {
                weaponHolderUi.Initialize(this);
            }
        }

        private void OnEnable()
        {
            loadout.OnWeaponSelectionRequest += OnWeaponSelectionRequest;
            selectButton.onClick.AddListener(OnSelectClicked);
            deselectButton.onClick.AddListener(InvokeBackClicked);
        }

        private void OnDisable()
        {
            loadout.OnWeaponSelectionRequest -= OnWeaponSelectionRequest;
            selectButton.onClick.RemoveListener(OnSelectClicked);
            deselectButton.onClick.RemoveListener(InvokeBackClicked);
        }

        private void OnWeaponSelectionRequest()
        {
            weaponHolderPanelGroup.Open();
        }

        public void DisplayWeaponUI(WeaponHolderUI holder)
        {
            if (CurrentWeaponHolderUI != null)
                return;
            
            CurrentWeaponHolderUI = holder;
            CurrentWeaponHolderUI.Select();
            Show();
            
            weaponIcon.sprite = CurrentWeaponHolderUI.Data.WeaponSprite;
            weaponName.text = CurrentWeaponHolderUI.Data.WeaponName;
            weaponDescription.text = CurrentWeaponHolderUI.Data.WeaponDescription;
            weaponType.text = CurrentWeaponHolderUI.Data.WeaponType;
            weaponDamage.text =
                (CurrentWeaponHolderUI.Data.BulletData.Damage.baseDamage * CurrentWeaponHolderUI.Data.BulletsPerShot).ToString(CultureInfo.InvariantCulture);

            float fireCooldown = 1 / CurrentWeaponHolderUI.Data.FireCooldown;
            weaponShootRate.text = $"{fireCooldown:F1}/s";
            weaponMagCapacity.text = (CurrentWeaponHolderUI.Data.MagCapacity / CurrentWeaponHolderUI.Data.BulletsPerShot).ToString();
        }

        protected override void OnShow()
        {
            bottomPanelGroup.Open();
        }
        
        protected override void OnHide()
        {
            bottomPanelGroup.Close();
        }

        public override void InvokeBackClicked()
        {
            Hide();
            DeselectWeapon();
        }

        private void OnSelectClicked()
        {
            if (CurrentWeaponHolderUI == null)
                return;
            
            weaponHolderPanelGroup.Close();
            loadout.ConfirmSelection(CurrentWeaponHolderUI.Data);
            
            Hide();
            DeselectWeapon();
        }

        private void DeselectWeapon()
        {
            if (CurrentWeaponHolderUI == null)
                return;
            
            CurrentWeaponHolderUI.Deselect();
            CurrentWeaponHolderUI = null;
        }
    }
}