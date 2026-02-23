using System;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public class HealableComponent : MonoBehaviour, IHealable, IHealth
    {
        public event Action OnHealed;

        public event IHealth.HealthChanged OnHealthChanged;
        public float MinHealth { get; }
        public float Health { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        public void SetMinHealth(float minHealth)
        {
            throw new NotImplementedException();
        }

        public bool IsAlive => Health > 0;
        
        public void Heal(float amount)
        {
            float previousHealth = Health;
            Health += amount;
            OnHealed?.Invoke();
            OnHealthChanged?.Invoke(previousHealth, Health, MaxHealth);
        }
    }
}