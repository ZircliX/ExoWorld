using System;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DamageableComponent : MonoBehaviour, IDamageable, IHealth
    {
        public event Action<float, float> OnHealthChanged;

        public bool IsInvincible { get; set; }
        public float MinHealth { get; private set; }
        public float Health { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        public bool IsAlive => Health > MinHealth;
        public void SetMinHealth(float minHealth)
        {
            MinHealth = minHealth;
        }

        public void TakeDamage(DamageInfo damage)
        {
            float previousHealth = Health;
            Health -= damage.baseDamage;
            if (Health <= MinHealth) Health = MinHealth;
            OnHealthChanged?.Invoke(previousHealth, Health);
        }
    }
}