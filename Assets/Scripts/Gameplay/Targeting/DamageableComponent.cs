using System;
using OverBang.ExoWorld.Core.Damage;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Targeting
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

        public void TakeDamage(DamageData damage)
        {
            float previousHealth = Health;
            Health -= damage.baseDamage;
            if (Health <= MinHealth) Health = MinHealth;
            OnHealthChanged?.Invoke(previousHealth, Health);
        }
    }
}