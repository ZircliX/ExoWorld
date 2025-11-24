using System;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DamageableAndHealableComponent : MonoBehaviour, IDamageable, IHealable
    {
        public event Action OnDamaged;
        public event Action OnHealed;
        
        public float Health { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        public bool IsAlive => Health > 0;
        
        public void Heal(float amount)
        {
            Health += amount;
            OnHealed?.Invoke();
        }

        public void TakeDamage(float damage)
        {
            Health -= damage;
            OnDamaged?.Invoke();
        }
    }
}