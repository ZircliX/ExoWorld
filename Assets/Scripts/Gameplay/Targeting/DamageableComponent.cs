using System;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DamageableComponent : MonoBehaviour, IDamageable, IHealth
    {
        public event Action<float, float> OnHealthChanged;
        
        public float Health { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        public bool IsAlive => Health > 0;
        public bool IsInvincible { get; set; }

        public void TakeDamage(DamageInfo damage)
        {
            float previousHealth = Health;
            Health -= damage.baseDamage;
            OnHealthChanged?.Invoke(previousHealth, Health);
        }
    }
}