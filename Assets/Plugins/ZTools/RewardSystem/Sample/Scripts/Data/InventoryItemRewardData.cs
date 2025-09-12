using UnityEngine;
using ZTools.RewardSystem.Core.Data;

namespace ZTools.RewardSystem.Sample.Data
{
    [CreateAssetMenu(fileName = "InventoryItemRewardData", menuName = "ZTools/RewardSystem/RewardData", order = 1)]
    public class InventoryItemRewardData : RewardData
    {
        [field: SerializeField] public string ItemName { get; private set; }
        [field: SerializeField] public string ItemDescription { get; private set; }
        [field: SerializeField] public int Quantity { get; private set; }
    }
}