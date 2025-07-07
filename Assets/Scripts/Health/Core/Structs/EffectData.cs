namespace Health.Core
{
    public readonly struct EffectData
    {
        public readonly float Amount;
        public readonly float PercentageAmount;
        public readonly float Delay;
        public readonly float Duration;
        public readonly float Steps;
        
        public EffectTypes EffectType => 
            Amount != 0 ? EffectTypes.Points : 
            PercentageAmount != 0 ? EffectTypes.Percentage : 
            EffectTypes.None;
        
        private EffectData(float amount, float percentageAmount, float delay, float duration, float steps)
        {
            Amount = amount;
            PercentageAmount = percentageAmount;
            Delay = delay;
            Duration = duration;
            Steps = steps;
        }
        
        public class Builder
        {
            private float _amount = 0;
            private float _percentageAmount = 0;
            private float _delay = 0;
            private float _duration = 0;
            private float _steps = 0;

            public Builder SetAmount(float amount)
            {
                _amount = amount;
                return this;
            }
            
            public Builder SetPercentageAmount(float percentage)
            {
                _percentageAmount = percentage;
                return this;
            }

            public Builder SetDelay(float delay)
            {
                _delay = delay;
                return this;
            }

            public Builder SetDuration(float duration)
            {
                _duration = duration;
                return this;
            }

            public Builder SetSteps(float steps)
            {
                _steps = steps;
                return this;
            }

            public EffectData Build()
            {
                return new EffectData(
                    _amount, 
                    _percentageAmount, 
                    _delay, 
                    _duration, 
                    _steps);
            }
        }
    }
}