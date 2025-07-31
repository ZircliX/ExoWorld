using System;
using System.Collections.Generic;
using LTX.ChanneledProperties.Priorities;
using LTX.Tools;
using UnityEngine;

namespace Health.Core
{
    public class HealthComponent : MonoBehaviour, IEffectReceiver
    {
        public List<EffectCommand> EffectCommands { get; protected set; }
        private DynamicBuffer<EffectCommand> _effectCommandsBuffer;
        public float MaxValue => MaxHealthPriority.Value;
        
        public float Health { get; private set; }
        public Priority<float> MaxHealthPriority { get; protected set; }
        
        public event Action<int> OnHealthChanged;
        protected void InvokeOnHealthChanged()
        {
            float totalCommandValues = 0f;
            _effectCommandsBuffer.Clear();
            _effectCommandsBuffer.CopyFrom(EffectCommands);

            for (int index = 0; index < _effectCommandsBuffer.Length; index++)
            {
                EffectCommand command = EffectCommands[index];

                bool isOver = Health + totalCommandValues + command.CurrentValue > MaxHealthPriority.Value;
                bool isUnder = Health + totalCommandValues + command.CurrentValue < 0f;
                
                if (isOver || isUnder)
                {
                    //Debug.Log($"isOver: {isOver}, isUnder: {isUnder}, Health: {Health}, TotalCommandValues: {totalCommandValues}, CommandValue: {command.CurrentValue}");
                    UnregisterEffectCommand(command);
                }

                totalCommandValues += command.CurrentValue;
            }

            float currentHealth = Mathf.Clamp(Health + totalCommandValues, 0f, MaxHealthPriority.Value);
            
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
            Health = MaxHealthPriority.Value;
            EffectCommands = new List<EffectCommand>(8);
            _effectCommandsBuffer = new DynamicBuffer<EffectCommand>(EffectCommands.Capacity);
        }

        public virtual void RegisterEffectCommand(EffectData effectData)
        {
            if (Health <= 0f)
            {
                Debug.LogWarning("Cannot apply effect to a dead entity.");
                return;
            }

            EffectCommand command = GetEffectCommand(effectData.EffectType);

            if (command.Equals(default))
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
                
                Health = Mathf.Clamp(Health + command.CurrentValue, 0, MaxHealthPriority.Value);
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
                    return new EffectCommand(new PointsMetricResolver());
                case EffectTypes.Percentage:
                    return new EffectCommand(new PercentageMetricResolver());
                default:
                    Debug.LogWarning($"Effect type {effectType} not implemented.");
                    return default;
            }
        }
        
        private bool CheckForDeath()
        {
            return Health <= 0f;
        }
    }
}