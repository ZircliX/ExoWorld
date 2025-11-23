using UnityEngine;
using ZTools.RewardSystem.Core.ZTools.RewardSystem.Core;

namespace ZTools.RewardSystem.Sample.ZTools.RewardSystem.Sample
{
    [CreateAssetMenu(fileName = "InventoryItemRewardData", menuName = "ZTools/RewardSystem/RewardData", order = 1)]
    public class InventoryItemRewardData : RewardData
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string ItemDescription { get; private set; }
        [field: SerializeField] public int Quantity { get; private set; }
    }
}