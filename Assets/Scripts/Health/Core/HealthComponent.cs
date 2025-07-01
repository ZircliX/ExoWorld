using System;
using Health.Structs;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace Health.Core
{
    public class HealthComponent : MonoBehaviour
    {
        public float Health => health;
        protected float health;
        public int HealthBarCount => healthBarCount;
        protected int healthBarCount;
        
        public Priority<float> MaxHealth { get; protected set; }
        public Priority<int> MaxHealthBarCount { get; protected set; }
        
        public event Action<float, float> OnHealthChanged;
        protected void InvokeOnHealthChanged() => OnHealthChanged?.Invoke(health, healthBarCount);

        protected HealthPointCommand healthPointCommand;
        protected HealthPercentageCommand healthPercentageCommand;
        protected HealthBarCommand healthBarCommand;

        protected virtual void Awake()
        {
            MaxHealth = new Priority<float>(100f);
            MaxHealthBarCount = new Priority<int>(1);
            
            healthPointCommand = new HealthPointCommand();
            healthPercentageCommand = new HealthPercentageCommand();
            healthBarCommand = new HealthBarCommand();
        }

        private void Start()
        {
            Initialize();
            InvokeOnHealthChanged();
        }
        
        protected void Initialize()
        {
            health = MaxHealth.Value;
            healthBarCount = MaxHealthBarCount.Value;
        }
        
        public virtual bool Damage(float damage)
        {
            if (health <= 0 && healthBarCount <= 0)
            {
                Debug.LogWarning("Cannot damage, health is already zero.");
                return true;
            }
            
            health -= damage;
            float remainingDamage = health < 0f ? -health : 0;

            // If health is below zero, we need to check if we have health bars left
            if (remainingDamage > 0)
            {
                healthBarCount--;
                
                if (healthBarCount > 0)
                {
                    health = MaxHealth.Value;
                    Damage(remainingDamage);
                }
            }
            
            // Death condition
            if (health <= 0f && healthBarCount <= 0)
            {
                health = 0f;
                healthBarCount = 0;
                
                InvokeOnHealthChanged();
                return true;
            }

            InvokeOnHealthChanged();
            return false;
        }
        
        public virtual void Heal(HealingMetrics healingMetrics, HealingData healingData)
        {
            HealingCommandArgs args = new HealingCommandArgs(
                () => health,
                newHealth => health = newHealth,
                () => healthBarCount,
                newCount => healthBarCount = newCount,
                OnHealthTick
            );
            
            HealingCommand command;
            switch (healingMetrics)
            {
                case HealingMetrics.HealthPoint:
                    command = healthPointCommand;
                    break;
                case HealingMetrics.HealthPercentage:
                    command = healthPercentageCommand;
                    break;
                case HealingMetrics.HealthBar:
                    command = healthBarCommand;
                    break;
                case HealingMetrics.None:
                default:
                    Debug.LogWarning($"Healing command for {healingMetrics} not found.");
                    return;
            }

            StartCoroutine(command.Execute(this, healingData, args));
        }

        protected virtual void OnHealthTick() => InvokeOnHealthChanged();
    }
}