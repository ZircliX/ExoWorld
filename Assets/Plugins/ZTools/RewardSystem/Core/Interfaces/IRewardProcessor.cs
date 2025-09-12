using ZTools.RewardSystem.Core.Data;

namespace ZTools.RewardSystem.Core.Interfaces
{
    internal interface IRewardProcessor
    {
        bool TryProcess(RewardData reward);
    }
}