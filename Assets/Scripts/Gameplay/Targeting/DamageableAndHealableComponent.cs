using Ami.BroAudio;
using OverBang.ExoWorld.Core.Damage;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public class DamageableAndHealableComponent : MonoBehaviour, IDamageable, IHealable, IHealth
    {
        [SerializeField] protected SoundID damagedSound;
        
        public event IHealth.HealthChanged OnHealthChanged;
        public float MinHealth { get; private set; }
        [field: SerializeField, ReadOnly] public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        public float Resistance { get; private set; }
        public bool IsAlive => Health > MinHealth;
        public void SetMinHealth(float minHealth)
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
            if (Health <= MinHealth) Health = MinHealth;
            OnHealthChanged?.Invoke(previousHealth, Health, MaxHealth);
        }

        public void Heal(float amount)
        {
            SetHealth(Mathf.Min(Health + amount, MaxHealth));
        }

        public void TakeDamage(DamageData damage)
        {
            BroAudio.Play(damagedSound, transform.position);
            SetHealth(Mathf.Max(Health - damage.baseDamage * (1f - Resistance), 0f));
        }
    }
}