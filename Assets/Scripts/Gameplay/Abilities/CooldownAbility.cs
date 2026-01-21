using System;
using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public sealed class CooldownAbility<TData> : IAbility<TData>
        where TData : AbilityData
    {
        private class CooldownComponent
        {
            private readonly float cooldownTime;
            private float cooldownRemaining;

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
                    cooldownRemaining -= deltaTime;
            }

            public void Start()
            {
                cooldownRemaining = cooldownTime;
            }
        }
        
        public TData Data { get; private set; }
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
            Data = data;
            cooldown = new CooldownComponent(data.Cooldown);

            MainStrategy = CreateStrategy(Data.MainData);
            AugmentStrategy1 = CreateStrategy(Data.AugmentData1);
            AugmentStrategy2 = CreateStrategy(Data.AugmentData2);
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
            
            currentDuration = Data.Duration;

            IsActive = true;
            ActiveStrategy.Begin(this);
        }

        public void Tick(float deltaTime)
        {
            cooldown.Tick(deltaTime);

            if (!IsActive)
                return;
            
            if (Data.Duration > 0)
            {
                currentDuration -= deltaTime;
                if (currentDuration <= 0)
                {
                    End();
                    return;
                }
            }
            
            ActiveStrategy.Tick(this, deltaTime);
        }

        public void End()
        {
            IsActive = false;
            cooldown.Start();
            
            ActiveStrategy.End(this);
        }
    }
}