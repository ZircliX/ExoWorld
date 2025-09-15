using UnityEngine;
using ZTools.Logger.Core.Interfaces;
using ZTools.RewardSystem.Core.Data;
using ZTools.RewardSystem.Core.Interfaces;

namespace ZTools.RewardSystem.Core
{
    public abstract class RewardProcessor<T> : MonoBehaviour, IRewardProcessor, ILogSource
        where T : RewardData
    {
        public virtual string Name => nameof(RewardProcessor<T>);
        
        protected virtual void OnEnable()
        {
            this.RegisterRewardProcessor();
        }
        
        protected virtual void OnDisable()
        {
            this.UnregisterRewardProcessor();
        }

        protected abstract bool TryProcess(T rewardData);
        
        [HideInCallstack]
        public virtual bool TryProcess(RewardData reward)
        {
            if (reward is T rewardT) return TryProcess(rewardT);
            return false;
        }
    }
}