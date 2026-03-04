using OverBang.ExoWorld.Core.Inventory;
using UnityEngine;
using ZTools.RewardSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    [CreateAssetMenu(menuName = "OverBang/RewardData/TrinititeRewardData",  fileName = "TrinititeRewardData")]
    public class TrinititeRewardData : RewardData
    {
        [field: SerializeField] public ItemData TrinititeData { get; private set; }
        [field: SerializeField] public int RewardQuantity { get; private set; }
    }
}