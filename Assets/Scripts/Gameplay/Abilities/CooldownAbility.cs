using System;
using OverBang.ExoWorld.Core.Abilities;
using OverBang.ExoWorld.Core.Characters;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public sealed class CooldownAbility<TData> : IAbility<TData>
        where TData : AbilityData
    {
        private class CooldownComponent
        {
            private readonly float cooldownTime;
            private float cooldownRemaining;

            public event Action OnAbilityCooldownEnded;
            public float Remaining => Mathf.Max(0, cooldownRemaining);
            public bool IsReady => cooldownRemaining <= 0;

            public CooldownComponent(float cooldownTime)
            {
                this.cooldownTime = cooldownTime;
                this.cooldownRemaining = 0;
            }

            public void Tick(float deltaTime)
            {
                if (cooldownRemaining > 0)
                {
                    cooldownRemaining -= deltaTime;
                    if (cooldownRemaining <= 0f)
                    {
                        // Clamp to zero and invoke the event only once when the cooldown finishes
                        cooldownRemaining = 0f;
                        OnAbilityCooldownEnded?.Invoke();
                    }
                }
            }

            public void Start()
            {
                cooldownRemaining = cooldownTime;
            }
        }
        
        public TData DataT { get; private set; }
        public AbilityData Data => DataT;
        public float Duration { get; private set; }
        public Action<IAbility> OnAbilityEnded { get; set; }
        public Action<IAbility> OnAbilityCooldownEnded { get; set; }
        public ICaster Caster { get; }
        public bool IsActive { get; private set; }
        public bool CanBeUsed => !IsActive && cooldown.IsReady;
        public AbilityAugmentState AugmentState { get; private set; } = AbilityAugmentState.Main;

        public IAbilityStrategy<TData> ActiveStrategy => AugmentState switch
        {
            AbilityAugmentState.Main => MainStrategy,
            AbilityAugmentState.Augment1 => AugmentStrategy1,
            AbilityAugmentState.Augment2 => AugmentStrategy2,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        public IAbilityStrategy<TData> MainStrategy { get; private set; }
        public IAbilityStrategy<TData> AugmentStrategy1 { get; private set; }
        public IAbilityStrategy<TData> AugmentStrategy2 { get; private set; }

        private readonly CooldownComponent cooldown;
        private float currentDuration;

        public CooldownAbility(TData data, ICaster caster)
        {
            Caster = caster;
            DataT = data;
            cooldown = new CooldownComponent(data.Cooldown);
            Duration = Data.Duration;

            cooldown.OnAbilityCooldownEnded += () => OnAbilityCooldownEnded?.Invoke(this);

            MainStrategy = CreateStrategy(DataT.MainData);
            AugmentStrategy1 = CreateStrategy(DataT.AugmentData1);
            AugmentStrategy2 = CreateStrategy(DataT.AugmentData2);
        }

        private IAbilityStrategy<TData> CreateStrategy(IAbilityStrategyData strategyData)
        {
            IAbilityStrategy<TData> strategy = strategyData.CreateStrategyFor<TData>();
            strategy.Initialize(this, Caster, strategyData);
            return strategy;
        }

        public void Begin()
        {
            if (!cooldown.IsReady)
                return;
            
            currentDuration = DataT.Duration;

            IsActive = true;
            ActiveStrategy.Begin(this);
        }

        public void Tick(float deltaTime)
        {
            cooldown.Tick(deltaTime);

            if (!IsActive)
                return;
            
            if (DataT.Duration > 0)
            {
                currentDuration -= deltaTime;
                if (currentDuration <= 0)
                {
                    End();
                    return;
                }
            }

            ActiveStrategy?.Tick(this, deltaTime);
        }

        public void End()
        {
            IsActive = false;
            cooldown.Start();
            
            ActiveStrategy.End(this);
            OnAbilityEnded?.Invoke(this);
        }
        
        public void SetDuration(float duration)
        {
            Debug.Log("Setting duration to " + duration);
            Duration = duration;
        }
    }
}