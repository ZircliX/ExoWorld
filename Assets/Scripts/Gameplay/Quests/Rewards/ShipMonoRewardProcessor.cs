using OverBang.ExoWorld.Gameplay.HUB;
using ZTools.RewardSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class ShipMonoRewardProcessor : MonoRewardProcessor<ShipRewardData>
    {
        protected override bool TryProcess(ShipRewardData rewardData)
        {
            if (TryGetComponent(out Ship ship))
            {
                ship.SetCanBeTriggered(true);
                return true;
            }

            return false;
        }
    }
}