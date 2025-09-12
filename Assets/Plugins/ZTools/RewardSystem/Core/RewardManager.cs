using System.Collections.Generic;
using ZTools.Logger.Core;
using ZTools.Logger.Core.Enums;
using ZTools.RewardSystem.Core.Data;
using ZTools.RewardSystem.Core.Interfaces;

namespace ZTools.RewardSystem.Core
{
    public static class RewardManager
    {
        public static readonly LogProvider LogProvider;
        private static readonly List<IRewardProcessor> RewardProcessors;

        static RewardManager()
        {
            RewardProcessors = new List<IRewardProcessor>();
            LogProvider = new LogProvider("Reward Manager", "RewardSystem");
        }
        
        public static void ClearProcessors()
        {
            RewardProcessors.Clear();
            LogProvider.Log("Cleared all registered reward processors.");
        }

        public static void RegisterRewardProcessor<T>(this RewardProcessor<T> processor)
            where T : RewardData
        {
            if (processor == null)
            {
                LogProvider.LogError("Attempted to register a null reward processor.");
                return;
            }

            RewardProcessors.Add(processor);
            LogProvider.Log($"Registered reward processor: {processor}");
        }
        
        public static void UnregisterRewardProcessor<T>(this RewardProcessor<T> processor)
            where T : RewardData
        {
            if (processor == null)
            {
                LogProvider.LogError("Attempted to unregister a null reward processor.");
                return;
            }

            if (RewardProcessors.Remove(processor))
            {
                LogProvider.Log($"Unregistered reward processor: {processor}");
            }
            else
            {
                LogProvider.LogWarning($"Reward processor with ID '{processor}' not found.");
            }
        }
        
        public static void ProcessReward(RewardData rewardData)
        {
            if (!rewardData)
            {
                LogProvider.LogError("Attempted to process null rewards.");
                return;
            }
            
            foreach (IRewardProcessor rewardProcessor in RewardProcessors)
            {
                if (rewardProcessor.TryProcess(rewardData))
                {
                    return;
                }
            }
            
            LogProvider.LogWarning($"Reward type {rewardData.GetType()} not supported by any processor");
        }
        
        public static void ProcessRewards(params RewardData[] rewardDatas)
        {
            if (rewardDatas == null)
            {
                LogProvider.LogError("Attempted to process null rewards.");
                return;
            }

            for (int index = 0; index < rewardDatas.Length; index++)
            {
                RewardData rewardReference = rewardDatas[index];
                ProcessReward(rewardReference);
            }
        }
    }
}