using System;
using System.Collections;
using UnityEngine;

namespace Health.Core
{
    public struct EffectCommand : IEquatable<EffectCommand>
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
            float targetValue = metricResolver.ResolveAmount(effectReceiver, effectData);
    
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
                // --- STEP-BASED LOGIC ---
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

        public bool Equals(EffectCommand other)
        {
            return Equals(metricResolver, other.metricResolver) && CurrentValue.Equals(other.CurrentValue);
        }

        public override bool Equals(object obj)
        {
            return obj is EffectCommand other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(metricResolver, CurrentValue);
        }
    }
}