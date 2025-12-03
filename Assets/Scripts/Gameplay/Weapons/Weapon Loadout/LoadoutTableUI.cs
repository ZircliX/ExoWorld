using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.GameName.Gameplay
{
    public class LoadoutTableUI : MonoBehaviour
    {
        [SerializeField] private LoadoutTable loadout;
        [SerializeField] private CanvasGroup weaponSelectGroup;
        [SerializeField] private CanvasGroup rankSelectGroup;
        [SerializeField] private Transform weaponHolder;
        
        [SerializeField, Space] private Image weaponIcon;
        [SerializeField] private TMP_Text weaponName;
        [SerializeField] private TMP_Text weaponType;
        [SerializeField] private TMP_Text weaponDescription;
        
        [SerializeField, Space] private TMP_Text weaponDamage;
        [SerializeField] private TMP_Text weaponShootRate;
        [SerializeField] private TMP_Text weaponMagCapacity;
        
        [SerializeField, Space] private TMP_Text weakPointsDamage;

        private WeaponData[] currentList;
        private WeaponRank currentRank;
        private int index;
        private GameObject currentDisplayedWeapon;

        private void OnEnable()
        {
            loadout.OnRankSelectionUIRequested += ShowRankUI;
            loadout.OnWeaponSelectionUIRequested += ShowWeaponSelection;
        }

        private void OnDisable()
        {
            loadout.OnRankSelectionUIRequested -= ShowRankUI;
            loadout.OnWeaponSelectionUIRequested -= ShowWeaponSelection;
        }
        
        public void OnRankPrimary() => loadout.OpenPrimary();
        public void OnRankSecondary() => loadout.OpenSecondary();

        public void OnSelectWeapon()
        {
            loadout.SelectWeapon(currentRank, currentList[index]);
            ShowWeaponPanel(false);
        }

        public void NextWeapon() => SelectRelative(+1);
        public void PrevWeapon() => SelectRelative(-1);

        private void SelectRelative(int step)
        {
            index = (index + step + currentList.Length) % currentList.Length;
            RefreshWeaponUI();
        }

        private void ShowRankUI(bool visible)
        {
            FadePanel(rankSelectGroup, visible);
        }

        private void ShowWeaponSelection(WeaponRank rank)
        {
            currentRank = rank;
            currentList = loadout.GetWeaponList(rank);
            index = 0;
            RefreshWeaponUI();

            FadePanel(rankSelectGroup, false);
            FadePanel(weaponSelectGroup, true);
        }

        private void RefreshWeaponUI()
        {
            DestroyImmediate(currentDisplayedWeapon);
            
            WeaponData w = currentList[index];
            weaponIcon.sprite = w.WeaponSprite;
            weaponName.text = w.WeaponName;
            weaponDescription.text = w.WeaponDescription;
            weaponType.text = w.name.ToString();
            weaponDamage.text = w.BulletData.Damage.baseDamage.ToString(CultureInfo.InvariantCulture);
            weaponShootRate.text = w.FireCooldown.ToString(CultureInfo.InvariantCulture);
            weaponMagCapacity.text = w.MagCapacity.ToString();

            currentDisplayedWeapon = Instantiate(w.ModelPrefab, weaponHolder);
        }

        private void ShowWeaponPanel(bool visible)
        {
            FadePanel(weaponSelectGroup, visible);
        }

        private void FadePanel(CanvasGroup group, bool visible)
        {
            group.DOFade(visible ? 1 : 0, 0.3f);
            group.interactable = visible;
            group.blocksRaycasts = visible;
        }
    }
}