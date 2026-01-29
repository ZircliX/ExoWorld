using DG.Tweening;
using OverBang.ExoWorld.Core.Inventory;
using OverBang.ExoWorld.Gameplay.Player.PlayerHUD;
using TMPro;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Upgrade
{
    public class UpgradeTableUi : MonoBehaviour
    {
        [Header("ui to assign :")]
        [SerializeField] private CanvasGroup upgradeGroup;
        [SerializeField] private UpgradeTable upgradeTable;
        
        [SerializeField] private TMP_Text playerTrinititeAmount;

        private void OnEnable()
        {
            upgradeTable.OnUpgradePanelRequest += ShowUi;
            UpgradeManager.Instance.OnPlayerTritiniteAmountChange += UpdateValue;
        }

        private void OnDisable()
        {
            upgradeTable.OnUpgradePanelRequest -= ShowUi;
            UpgradeManager.Instance.OnPlayerTritiniteAmountChange -= UpdateValue;
        }

        private void ShowUi(bool visible)
        {
            FadePanel(upgradeGroup, visible);
            HUD.Instance.ChangeHudState(visible);
            UpgradeManager.Instance.RefreshTable();
            UpdateValue(ResourcesInventory.Instance.GetItemQuantity("Trinitite"));
        }
        
        private void FadePanel(CanvasGroup group, bool visible)
        {
            group.DOFade(visible ? 1 : 0, 0.3f);
            group.interactable = visible;
            group.blocksRaycasts = visible;
        }
        private void UpdateValue(int value)
        {
            playerTrinititeAmount.text = $"trinitite : {value}";
        }
    }
}