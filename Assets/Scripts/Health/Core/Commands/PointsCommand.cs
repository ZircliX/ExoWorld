using System.Collections;
using UnityEngine;

namespace Health.Core
{
    public class PointsCommand : EffectCommand
    {
        public override IEnumerator Execute(IEffectContext effectContext, EffectData effectData)
        {
            float baseValue = effectContext.CurrentValue;
            float targetValue = baseValue + effectData.Amount;
    
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
                float interval = effectData.Duration / effectData.Steps;
                float amountPerStep = effectData.Amount / effectData.Steps;

                for (int i = 0; i < effectData.Steps; i++)
                {
                    yield return new WaitForSeconds(interval);
                    
                    effectContext.ApplyEffectTick(this, amountPerStep);
                }
            }
            else
            {
                float healingPerSecond = effectData.Amount / effectData.Duration;
                float timer = 0;

                while (timer < effectData.Duration)
                {
                    float amountThisFrame = healingPerSecond * Time.deltaTime;
                    effectContext.ApplyEffectTick(this, amountThisFrame);
            
                    timer += Time.deltaTime;
                    yield return null;
                }
            }
            
            //effectContext.SetValue(this, targetValue);
        }
    }
}