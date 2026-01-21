using OverBang.GameName.Core;
using TMPro;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class UpgradeUi : MonoBehaviour
    {
        [Header("ui to assign :")]
        [SerializeField] private TextMeshProUGUI upgradeName;
        [SerializeField] private TextMeshProUGUI upgradeDescription;
        [SerializeField] private TextMeshProUGUI upgradeLevel;
        [SerializeField] private TextMeshProUGUI upgradePrice;
        [SerializeField] private TextMeshProUGUI upgradeBonus;

        public void Refresh(RuntimeUpgradeData data)
        {
            switch (data.upgradeData.UpgradeType)
            {
                case UpgradeType.Health : 
                    upgradeBonus.text = $"Bonus Actuel : {data.finalBonus}";
                    break;
                case UpgradeType.Damage :
                case UpgradeType.MaxMagCap :
                case UpgradeType.Resistance :
                    upgradeBonus.text = $"Bonus Actuel : {data.finalBonus}%";
                    break;
            }
            
            upgradeName.text = data.upgradeData.UpgradeName;
            upgradeDescription.text = data.upgradeData.UpgradeDesc;
            upgradeLevel.text = $"Niveau : {data.level}";
            upgradePrice.text = $"Prix : {data.cost}";
        }
    }
}