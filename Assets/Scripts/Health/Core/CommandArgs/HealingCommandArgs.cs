using System;

namespace Health.Core
{
    public readonly struct HealingCommandArgs
    {
        public readonly Func<float> getHealth;
        public readonly Action<float> setHealth;
        
        public readonly Func<int> getHealthBarCount;
        public readonly Action<int> setHealthBarCount;
        
        public readonly Action onTick;
        
        public HealingCommandArgs(Func<float> getHealth, Action<float> setHealth, Func<int> getHealthBarCount, Action<int> setHealthBarCount, Action onTick)
        {
            this.getHealth = getHealth;
            this.setHealth = setHealth;
            this.getHealthBarCount = getHealthBarCount;
            this.setHealthBarCount = setHealthBarCount;
            this.onTick = onTick;
        }
    }
}