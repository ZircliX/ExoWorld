using System;
using Ami.BroAudio;
using OverBang.GameName.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DamageableAndHealableComponent : MonoBehaviour, IDamageable, IHealable, IHealth
    {
        [SerializeField] protected SoundID damagedSound;
        
        public event Action<float, float> OnHealthChanged;
        [field: SerializeField, ReadOnly] public float Health { get; private set; }
        public float MaxHealth { get; private set; }
        public float Resistance { get; private set; }
        public bool IsAlive => Health > 0;
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
            OnHealthChanged?.Invoke(previousHealth, Health);
        }

        public void Heal(float amount)
        {
            SetHealth(Mathf.Min(Health + amount, MaxHealth));
        }

        public void TakeDamage(DamageInfo damage)
        {
            BroAudio.Play(damagedSound, transform.position);
            SetHealth(Mathf.Max(Health - damage.baseDamage * (1f - Resistance), 0f));
        }
    }
}