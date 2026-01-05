using System;
using Ami.BroAudio;
using OverBang.GameName.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class DamageableAndHealableComponent : MonoBehaviour, IDamageable, IHealable
    {
        [SerializeField] protected SoundID damagedSound;
        
        public event Action OnDamaged;
        public event Action OnHealed;
        
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
            Health = health;
            OnHealed?.Invoke();
        }

        public void Heal(float amount)
        {
            Health += amount;
            OnHealed?.Invoke();
        }

        public void TakeDamage(DamageInfo damage)
        {
            Health -= damage.baseDamage;
            BroAudio.Play(damagedSound, transform.position);
            OnDamaged?.Invoke();
        }
    }
}