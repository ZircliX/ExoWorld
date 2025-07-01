using System.Collections;
using Health.Structs;
using UnityEngine;

namespace Health.Core
{
    public class HealthPointCommand : HealingCommand
    {
        public override IEnumerator Execute(HealthComponent healthComponent, HealingData healingData, HealingCommandArgs healingCommandArgs)
        {
            if (healingData.Delay > 0)
            {
                yield return new WaitForSeconds(healingData.Delay);
            }
            if (healingData.Duration <= 0)
            {
                // Instant heal
                float currentHealth = healingCommandArgs.getHealth();
                float newHealth = currentHealth + healingData.Amount;
                
                healingCommandArgs.setHealth(newHealth);
                healingCommandArgs.onTick?.Invoke();
                
                yield break;
            }
            
            float healingPerSecond = healingData.Amount / healingData.Duration;
            float timer = 0;

            while (timer < healingData.Duration)
            {
                float currentHealth = healingCommandArgs.getHealth();
                float newHealth = currentHealth + healingPerSecond * Time.deltaTime;

                healingCommandArgs.setHealth(newHealth);
                
                timer += Time.deltaTime;
                healingCommandArgs.onTick?.Invoke();
                
                yield return null;
            }
        }
    }
    
    public class HealthPercentageCommand : HealingCommand
    {
        public override IEnumerator Execute(HealthComponent healthComponent, HealingData healingData, HealingCommandArgs healingCommandArgs)
        {
            if (healingData.Delay > 0)
            {
                yield return new WaitForSeconds(healingData.Delay);
            }
            if (healingData.Duration <= 0)
            {
                // Instant heal
                float currentHealth = healingCommandArgs.getHealth();
                float newHealth = currentHealth + (healingData.Amount / 100f) * healthComponent.MaxHealth.Value;
                healingCommandArgs.setHealth(newHealth);
                healingCommandArgs.onTick?.Invoke();
                yield break;
            }
            
            float healingPerSecond = (healingData.Amount / 100f) * healthComponent.MaxHealth.Value / healingData.Duration;
            float timer = 0;

            while (timer < healingData.Duration)
            {
                float currentHealth = healingCommandArgs.getHealth();
                float newHealth = currentHealth + healingPerSecond * Time.deltaTime;
                healingCommandArgs.setHealth(newHealth);
                
                timer += Time.deltaTime;
                healingCommandArgs.onTick?.Invoke();
                yield return null;
            }
        }
    }
    
    public class HealthBarCommand : HealingCommand
    {
        public override IEnumerator Execute(HealthComponent healthComponent, HealingData healingData, HealingCommandArgs healingCommandArgs)
        {
            if (healingData.Delay > 0)
            {
                yield return new WaitForSeconds(healingData.Delay);
            }
            if (healingData.Duration <= 0)
            {
                // Instant heal
                int currentHealthBarCount = healingCommandArgs.getHealthBarCount();
                int newHealthBarCount = currentHealthBarCount + (int)healingData.Amount;
                healingCommandArgs.setHealthBarCount(newHealthBarCount);
                healingCommandArgs.onTick?.Invoke();
                yield break;
            }
            
            int healingPerSecond = (int)(healingData.Amount / healingData.Duration);
            float timer = 0;

            while (timer < healingData.Duration)
            {
                int currentHealthBarCount = healingCommandArgs.getHealthBarCount();
                int newHealthBarCount = currentHealthBarCount + healingPerSecond;
                
                healingCommandArgs.setHealthBarCount(newHealthBarCount);
                
                timer += Time.deltaTime;
                healingCommandArgs.onTick?.Invoke();
                yield return null;
            }
        }
    }
}