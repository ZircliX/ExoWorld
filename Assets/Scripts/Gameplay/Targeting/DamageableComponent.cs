using System;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DamageableComponent : MonoBehaviour, IDamageable
    {
        public event Action OnDamaged;
        
        public float Health { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        public bool IsAlive => Health > 0;
        
        public void TakeDamage(float damage)
        {
            Health -= damage;
            OnDamaged?.Invoke();
        }
    }
}