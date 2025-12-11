using DG.Tweening;
using OverBang.GameName.Core;
using TMPro;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class UpgradeTableUi : MonoBehaviour
    {
        [Header("ui to assign :")]
        [SerializeField] private CanvasGroup upgradeGroup;
        [SerializeField] private UpgradeTable upgradeTable;
        
        [SerializeField] private TextMeshProUGUI playerTrinititeAmount;

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

        public void ShowUi(bool visible)
        {
            FadePanel(upgradeGroup, visible);
            HUD.Instance.ChangeHudState(visible);
            UpgradeManager.Instance.RefreshTable();
            UpdateValue(PlayerInventory.Trinitite);
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