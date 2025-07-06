using System;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;

namespace Health.Core
{
    public class HealthComponent : MonoBehaviour, IEffectContext
    {
        protected float health;
        
        public Priority<float> MaxHealthPriority { get; protected set; }
        
        public event Action<float> OnHealthChanged;
        protected void InvokeOnHealthChanged()
        {
            CheckForDeath();
            OnHealthChanged?.Invoke(health);
        }

        #region IEffectContexts

        public float CurrentValue => health;
        public float MaxValue => MaxHealthPriority.Value;
        
        public void ApplyEffectTick(IEffectCommand sender, float effectValue)
        {
            health += effectValue;
            health = Mathf.Clamp(health, 0f, MaxHealthPriority.Value);
            InvokeOnHealthChanged();
        }
        
        public void SetValue(IEffectCommand sender, float value)
        {
            health = Mathf.Min(value, MaxHealthPriority.Value);
            InvokeOnHealthChanged();
        }

        #endregion

        private void Awake()
        {
            Initialize();
            InvokeOnHealthChanged();
        }
        
        protected void Initialize()
        {
            MaxHealthPriority = new Priority<float>(100f);
            
            health = MaxHealthPriority.Value;
        }
        
        public virtual void ApplyEffect(EffectData effectData)
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

            StartCoroutine(command.Execute(this, effectData));
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