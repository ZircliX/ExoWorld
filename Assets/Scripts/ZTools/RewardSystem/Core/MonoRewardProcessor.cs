using Unity.Netcode;
using UnityEngine;
using ZTools.Logger.Core;

namespace ZTools.RewardSystem.Core
{
    public abstract class MonoRewardProcessor<T> : MonoBehaviour, IRewardProcessor, ILogSource
        where T : RewardData
    {
        public virtual string Name => nameof(MonoRewardProcessor<T>);
        
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
    
    public abstract class NetworkRewardProcessor<T> : NetworkBehaviour, IRewardProcessor, ILogSource
        where T : RewardData
    {
        public virtual string Name => nameof(NetworkRewardProcessor<T>);
        
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