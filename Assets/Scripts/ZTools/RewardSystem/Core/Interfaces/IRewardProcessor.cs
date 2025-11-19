using ZTools.RewardSystem.Core.ZTools.RewardSystem.Core.Data;

namespace ZTools.RewardSystem.Core.ZTools.RewardSystem.Core.Interfaces
{
    internal interface IRewardProcessor
    {
        bool TryProcess(RewardData reward);
    }
}