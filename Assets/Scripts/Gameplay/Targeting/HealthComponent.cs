using Ami.BroAudio;
using OverBang.ExoWorld.Core.Damage;
using Sirenix.OdinInspector;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public class HealthComponent : MonoBehaviour, IDamageable, IHealable, IHealth
    {
        [SerializeField] protected SoundID damagedSound;
        [SerializeField] protected SoundID killedSound;
        
        [field: SerializeField] public Transform DamageTarget { get; private set; }
        
        public RuntimeDamageData LastDamageData { get; private set; }
        
        public event IHealth.HealthChanged OnHealthChanged;
        public float MinHealth { get; private set; }
        [field: SerializeField, ReadOnly] public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        public float Resistance { get; private set; }
        public bool IsAlive => Health > 0;
        public void SetMinHealthRpc(float minHealth)
        {
            MinHealth = minHealth;
        }

        public bool IsInvincible { get; set; }

        public void Initialize(float maxHealth,  float resistance)
        {
            MaxHealth = maxHealth;
            Health = MaxHealth;
            Resistance = resistance;
        }

        public void SetHealth(float health)
        {
            float previousHealth = Health;
            Health = health;
            if (Health <= 0)
            {
                BroAudio.Play(killedSound, transform.position);
                Health = 0;
            }
            OnHealthChanged?.Invoke(previousHealth, Health, MaxHealth);
        }

        public void Heal(float amount)
        {
            SetHealth(Mathf.Min(Health + amount, MaxHealth));
        }

        public void TakeDamage(RuntimeDamageData damage)
        {
            if (!IsAlive || IsInvincible) return;
            
            if (damagedSound.IsValid())
                BroAudio.Play(damagedSound, transform.position);
            LastDamageData = damage;
            
            DamageRpc(damage.finalDamage);
        }

        [Rpc(SendTo.Owner)]
        private void DamageRpc(float damage)
        {
            SetHealth(Mathf.Max(Health - damage * (1f - Resistance), 0f));
        }
    }
}