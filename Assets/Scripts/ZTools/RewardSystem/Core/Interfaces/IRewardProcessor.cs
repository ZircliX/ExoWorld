namespace ZTools.RewardSystem.Core
{
    internal interface IRewardProcessor
    {
        bool TryProcess(RewardData reward);
    }
}