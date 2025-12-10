using OverBang.GameName.Core;
using UnityEngine;
using ZTools.RewardSystem.Core;

namespace OverBang.GameName.Gameplay
{
    public class TrinititeRewardProcessor : RewardProcessor<TrinititeRewardData>
    {
        protected override bool TryProcess(TrinititeRewardData rewardData)
        {
            PlayerInventory.ReceiveTrinitite(rewardData.TrinititeReward);
            return true;
        }
    }
}