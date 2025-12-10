namespace ZTools.RewardSystem.Core
{
    public interface IRewardProcessor
    {
        bool TryProcess(RewardData reward);
    }
}