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

        private void OnEnable()
        {
            if (Health < MaxHealth)
                Heal(MaxHealth);
        }

        public void Initialize(float maxHealth,  float resistance)
        {
            float bonusHealth = UpgradeManager.Instance.GetRuntimeUpgrade(UpgradeType.Health);
            MaxHealth = maxHealth + bonusHealth;
            Resistance = resistance;
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