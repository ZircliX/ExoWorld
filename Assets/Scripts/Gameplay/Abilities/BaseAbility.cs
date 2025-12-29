using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public abstract class BaseAbility<TData> : IAbility<TData>
        where TData : AbilityData
    {
        public TData Data { get; protected set; }
        public GameObject Owner { get; protected set; }
        public bool IsActive { get; protected set; }

        public CooldownComponent Cooldown { get; protected set; }

        protected BaseAbility(TData data, GameObject owner)
        {
            Data = data;
            Owner = owner;
        }

        public virtual void Begin()
        {
            if (!Cooldown.IsReady)
                return;

            IsActive = true;
            OnBegin();
        }

        public virtual void Tick(float deltaTime)
        {
            Cooldown.Tick(deltaTime);
            
            if (IsActive)
                OnTick(deltaTime);
        }

        public virtual void End()
        {
            IsActive = false;
            OnEnd();
            Cooldown.Start();
        }

        protected abstract void OnBegin();
        protected abstract void OnTick(float deltaTime);
        protected abstract void OnEnd();
    }
}