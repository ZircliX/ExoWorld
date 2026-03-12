using OverBang.ExoWorld.Core.Damage;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public class DamageableComponent : MonoBehaviour, IDamageable, IHealth
    {
        public event IHealth.HealthChanged OnHealthChanged;

        public bool IsInvincible { get; set; }
        public float MinHealth { get; private set; }
        public float Health { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        public bool IsAlive => Health > MinHealth;
        public void SetMinHealth(float minHealth)
        {
            MinHealth = minHealth;
        }
        
        [field: SerializeField] public Transform DamageTarget { get; private set; }

        public void TakeDamage(RuntimeDamageData damage)
        {
            float previousHealth = Health;
            Health -= damage.finalDamage;
            if (Health <= MinHealth) Health = MinHealth;
            OnHealthChanged?.Invoke(previousHealth, Health, MaxHealth);
        }
    }
}