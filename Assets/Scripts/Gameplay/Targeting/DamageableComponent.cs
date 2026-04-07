using OverBang.ExoWorld.Core.Damage;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public class DamageableComponent : NetworkBehaviour, IDamageable, IHealth
    {
        [field: SerializeField] public Transform DamageTarget { get; private set; }
        public event IHealth.HealthChanged OnHealthChanged;

        public bool IsInvincible { get; set; }
        public float MinHealth { get; private set; }
        public float Health { get; private set; }
        [field: SerializeField] public float MaxHealth { get; private set; }
        public bool IsAlive => Health > MinHealth;
        
        [Rpc(SendTo.Owner)]
        public void SetMinHealthRpc(float minHealth)
        {
            MinHealth = minHealth;
        }

        public void TakeDamage(RuntimeDamageData damage)
        {
            float previousHealth = Health;
            Health -= damage.finalDamage;
            HealthChangedRpc(Health, previousHealth);
            if (Health <= MinHealth) Health = MinHealth;
        }

        [Rpc(SendTo.Owner)]
        private void HealthChangedRpc(float health, float previousHealth)
        {
            Health = health;
            OnHealthChanged?.Invoke(previousHealth, Health, MaxHealth);
        }
    }
}