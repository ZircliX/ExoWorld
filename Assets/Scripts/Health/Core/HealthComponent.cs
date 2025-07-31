using System;
using System.Collections.Generic;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace Health.Core
{
    public class HealthComponent : MonoBehaviour, IEffectReceiver
    {
        public List<EffectCommand> EffectCommands { get; protected set; }
        public float MaxValue => MaxHealthPriority.Value;
        
        protected float health;
        public Priority<float> MaxHealthPriority { get; protected set; }
        
        public event Action<int> OnHealthChanged;
        protected void InvokeOnHealthChanged()
        {
            float totalCommandValues = 0f;
            foreach (EffectCommand command in EffectCommands)
            {
                totalCommandValues += command.CurrentValue;
            }
            
            float currentHealth = Mathf.Clamp(health + totalCommandValues, 0f, MaxHealthPriority.Value);
            
            //CheckForDeath();
            OnHealthChanged?.Invoke(Mathf.RoundToInt(currentHealth));
        }

        private void Awake()
        {
            Initialize();
            InvokeOnHealthChanged();
        }
        
        protected void Initialize()
        {
            MaxHealthPriority = new Priority<float>(100f);
            health = MaxHealthPriority.Value;
            EffectCommands = new List<EffectCommand>(8);
        }

        public virtual void RegisterEffectCommand(EffectData effectData)
        {
            if (health <= 0f)
            {
                Debug.LogWarning("Cannot apply effect to a dead entity.");
                return;
            }

            EffectCommand command = GetEffectCommand(effectData.EffectType);

            if (command == null)
            {
                Debug.LogWarning($"No command found for effect type: {effectData.EffectType}");
                return;
            }

            EffectCommands.Add(command);
            StartCoroutine(command.Execute(this, effectData));
        }

        public void UnregisterEffectCommand(EffectCommand command)
        {
            if (EffectCommands.Contains(command))
            {
                EffectCommands.Remove(command);
                
                health += Mathf.Clamp(health + command.CurrentValue, 0, MaxHealthPriority.Value);
                InvokeOnHealthChanged();
            }
        }

        public void OnEffectTick(EffectCommand command)
        {
            InvokeOnHealthChanged();
        }
        
        protected virtual EffectCommand GetEffectCommand(EffectTypes effectType)
        {
            switch (effectType)
            {
                case EffectTypes.Points:
                    return new PointsCommand();
                case EffectTypes.Percentage:
                    return new PercentageCommand();
                default:
                    Debug.LogWarning($"Effect type {effectType} not implemented.");
                    return null;
            }
        }
        
        private bool CheckForDeath()
        {
            return health <= 0f;
        }
    }
}