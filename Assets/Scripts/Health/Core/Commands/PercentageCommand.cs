using System.Collections;
using UnityEngine;

namespace Health.Core
{
    public class PercentageCommand : EffectCommand
    {
        public override float CurrentValue { get; protected set; }

        public override IEnumerator Execute(IEffectReceiver effectReceiver, EffectData effectData)
        {
            CurrentValue = 0;
            
            float totalEffectAmount = (effectData.PercentageAmount / 100f) * effectReceiver.MaxValue;
            float targetValue = totalEffectAmount;

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
                float intervalPerStep = effectData.Duration / effectData.Steps;
                float amountPerStep = totalEffectAmount / effectData.Steps;

                for (int i = 0; i < effectData.Steps; i++)
                {
                    CurrentValue += amountPerStep;
                    effectReceiver.OnEffectTick(this);
                    
                    yield return new WaitForSeconds(intervalPerStep);
                }
            }
            else
            {
                // --- CONTINUOUS (PER-FRAME) LOGIC ---
                float amountPerSecond = totalEffectAmount / effectData.Duration;
                float timer = 0f;

                while (timer < effectData.Duration)
                {
                    float amountThisFrame = amountPerSecond * Time.deltaTime;
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