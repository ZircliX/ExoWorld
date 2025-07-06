using System.Collections;
using UnityEngine;

namespace Health.Core
{
    public class PercentageCommand : EffectCommand
    {
        public override IEnumerator Execute(IEffectContext effectContext, EffectData effectData)
        {
            float totalEffectAmount = (effectData.PercentageAmount / 100f) * effectContext.MaxValue;
            float baseValue = effectContext.CurrentValue;
            float targetValue = baseValue + totalEffectAmount;

            if (effectData.Delay > 0)
            {
                yield return new WaitForSeconds(effectData.Delay);
            }

            if (effectData.Duration <= 0)
            {
                effectContext.SetValue(this, targetValue);
                yield break;
            }

            if (effectData.Steps > 0)
            {
                // --- STEP-BASED LOGIC ---
                float intervalPerStep = effectData.Duration / effectData.Steps;
                float amountPerStep = totalEffectAmount / effectData.Steps;

                for (int i = 0; i < effectData.Steps; i++)
                {
                    effectContext.ApplyEffectTick(this, amountPerStep);
                    
                    if (i < effectData.Steps - 1)
                    {
                        yield return new WaitForSeconds(intervalPerStep);
                    }
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
                    effectContext.ApplyEffectTick(this, amountThisFrame);

                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            
            //effectContext.SetValue(this, targetValue);
        }   
    }
}