using System;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Upgrade;
using OverBang.ExoWorld.Gameplay.Loadout;
using OverBang.ExoWorld.Gameplay.Movement;
using OverBang.ExoWorld.Gameplay.Targeting;
using OverBang.ExoWorld.Gameplay.Upgrade;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Player
{
    public class PlayerEntity : MonoBehaviour, IPlayerComponent, ITargetable, ISpeedTarget, IDamageable, IHealable, IHealth
    {
        public PlayerController Controller { get; private set; }
        private CharacterData characterData;

        public WeaponController WeaponController { get; private set; }

        private PlayerMovement pm;
        private float currentSlowTimer;

        private void OnEnable()
        {
            UpgradeManager.Instance.OnUpgrade += Initialize;
        }

        private void OnDisable()
        {
            UpgradeManager.Instance.OnUpgrade -= Initialize;
        }

        public void OnSync(PlayerRuntimeContext context)
        {
            characterData = context.playerCharacterData;
            Controller = context.playerController;

            WeaponController = Controller.GetComponent<WeaponController>();
            pm = GetComponent<PlayerMovement>();
            
            Initialize();
        }
        
        private void Initialize()
        {
            Controller.LocalGamePlayer.SetMaxHealth(characterData.BaseStats.Health + UpgradeManager.Instance.GetRuntimeUpgrade(UpgradeType.Health));
            Controller.LocalGamePlayer.SetHealth(Controller.LocalGamePlayer.MaxHealth);
            //characterData.BaseStats.Resistance + UpgradeManager.Instance.GetRuntimeUpgrade(UpgradeType.Resistance));
        }

        public event Action<bool> OnTargeted;
        
        public TargetPriority Priority => TargetPriority.High;
        public bool IsTargetable { get; private set; } = true;
        public void SetTargetable(bool state)
        {
            IsTargetable = state;
            OnTargeted?.Invoke(IsTargetable);
        }

        public void ApplySpeed(float speedPercentage, float duration)
        {
            currentSlowTimer = duration;
            pm.SetMovementSpeedMultiplier(1 + speedPercentage);
        }

        private void Update()
        {
            if (currentSlowTimer > 0)
                currentSlowTimer -= Time.deltaTime;
            
            if (currentSlowTimer <= 0)
            {
                pm.SetMovementSpeedMultiplier(1);
            }
        }

        public void TakeDamage(RuntimeDamageData damage)
        {
            if (Health - damage.finalDamage >= MaxHealth)
            {
                Controller.LocalGamePlayer.SetHealth(Health - damage.finalDamage);
            }
        }

        public void Heal(float amount)
        {
            float health = Health + amount;
            Controller.LocalGamePlayer.SetHealth(Mathf.Min(health, MaxHealth));
        }

        public event IHealth.HealthChanged OnHealthChanged;
        public float MinHealth { get; private set; }
        public float Health => Controller.LocalGamePlayer.Health;
        public float MaxHealth => Controller.LocalGamePlayer.MaxHealth;
        public void SetMinHealth(float minHealth)
        {
            MinHealth = minHealth;
        }
    }
}