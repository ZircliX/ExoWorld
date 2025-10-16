using UnityEngine;
using ZTools.RewardSystem.Core;

namespace OverBang.GameName.Quests.Quests.Rewards
{
    public class ShipRewardProcessor : RewardProcessor<ShipRewardData>
    {
        [SerializeField] private GameObject ship;

        private void Start()
        {
            ship.SetActive(false);
        }

        protected override bool TryProcess(ShipRewardData rewardData)
        {
            ship.SetActive(true);
            return true;
        }
    }
}