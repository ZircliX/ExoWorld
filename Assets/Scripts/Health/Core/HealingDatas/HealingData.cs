namespace Health.Structs
{
    public readonly struct HealingData
    {
        public readonly float Amount;
        public readonly float Delay;
        public readonly float Duration;
        public readonly float Steps;
        
        public HealingData(float amount, float duration, float steps, float delay)
        {
            Amount = amount;
            Delay = delay;
            Duration = duration;
            Steps = steps;
        }
        
        public HealingData(float amount, float duration, float steps)
        {
            Amount = amount;
            Delay = 0;
            Duration = duration;
            Steps = steps;
        }
        
        public HealingData(float amount, float duration)
        {
            Amount = amount;
            Delay = 0;
            Duration = duration;
            Steps = 1;
        }
        
        public HealingData(float amount)
        {
            Amount = amount;
            Delay = 0;
            Duration = 0;
            Steps = 1;
        }
    }
}