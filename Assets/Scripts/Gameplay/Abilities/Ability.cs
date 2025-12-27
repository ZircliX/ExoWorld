using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public abstract class Ability<TData> : IAbility<TData>
        where TData : AbilityData
    {
        public AbilityData Data { get; private set; }
        public TData TypedData { get; private set; }
        public GameObject Owner { get; private set; }

        public bool IsActive { get; protected set; }
        public float CooldownTimer { get; protected set; }
        protected float DurationTimer { get; set; }

        public virtual bool Initialize(AbilityData data, GameObject owner)
        {
            if (data is TData typedData)
            {
                TypedData = typedData;
                Data = data;
                Owner = owner;
                return true;
            }

            return false;
        }

        public virtual void Begin()
        {
            if (CooldownTimer > 0) return;

            IsActive = true;
            DurationTimer = Data.Duration;
            OnBegin();
            
            if (Data.Duration <= 0)
            {
                End();
            }
        }

        protected abstract void OnBegin();

        public virtual void OnTick(float deltaTime)
        {
            if (CooldownTimer > 0)
            {
                CooldownTimer -= deltaTime;
            }

            if (!IsActive) return;

            DurationTimer -= deltaTime;
            if (DurationTimer <= 0 && Data.Duration > 0)
            {
                End();
            }
            
            OnUpdate(deltaTime);
        }

        protected virtual void OnUpdate(float deltaTime) { }

        public virtual void End()
        {
            if (!IsActive) return;

            IsActive = false;
            CooldownTimer = Data.Cooldown;
            OnEnd();
        }

        protected abstract void OnEnd();
    }
}