using System.Collections;
using UnityEngine;

namespace OverBang.GameName.Health
{
    public class EffectCommand
    {
        private readonly IEffectMetricResolver metricResolver;
        public float CurrentValue { get; private set; }
        
        public EffectCommand(IEffectMetricResolver metricResolver)
        {
            this.metricResolver = metricResolver;
            CurrentValue = 0f;
        }

        public IEnumerator Execute(IEffectReceiver effectReceiver, EffectData effectData)
        {
            float targetValue = metricResolver.Resolve(effectReceiver, effectData);
            
            if (effectData.Delay > 0)
            {
                yield return new WaitForSeconds(effectData.Delay);
            }
    
            if (effectData.Duration <= 0)
            {
                CurrentValue = targetValue;
                effectReceiver.OnEffectTick(this);
                yield break;
            }
            
            if (effectData.Steps > 0)
            {
                float interval = effectData.Duration / effectData.Steps;
                float amountPerStep = effectData.Amount / effectData.Steps;

                for (int i = 0; i < effectData.Steps; i++)
                {
                    CurrentValue += amountPerStep;
                    effectReceiver.OnEffectTick(this);
                    
                    yield return new WaitForSeconds(interval);
                }
            }
            else
            {
                float healingPerSecond = effectData.Amount / effectData.Duration;
                float timer = 0;

                while (timer < effectData.Duration)
                {
                    float amountThisFrame = healingPerSecond * Time.deltaTime;
                    CurrentValue += amountThisFrame;
                    effectReceiver.OnEffectTick(this);
            
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            
            CurrentValue = targetValue;
            effectReceiver.UnregisterEffectCommand(this);
        }
    }
}