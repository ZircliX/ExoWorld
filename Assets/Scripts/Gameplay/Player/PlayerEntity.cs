using System;
using System.Collections.Generic;
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
        private struct SpeedEffect
        {
            public float speedMultiplier;
            public float duration;
            public float elapsedTime;
            public bool isPermanent;

            public SpeedEffect(SpeedEffect effect)
            {
                speedMultiplier = effect.speedMultiplier;
                duration = effect.duration;
                elapsedTime = effect.elapsedTime;
                isPermanent = effect.isPermanent;
            }
        }
        
        public PlayerController Controller { get; private set; }
        private CharacterData characterData;

        public WeaponController WeaponController { get; private set; }

        private PlayerMovement pm;
        private Dictionary<string, SpeedEffect> activeEffects;

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
            activeEffects = new Dictionary<string, SpeedEffect>(8);
        }

        public event Action<bool> OnTargeted;
        
        public TargetPriority Priority => TargetPriority.High;
        public bool IsTargetable { get; private set; } = true;
        public void SetTargetable(bool state)
        {
            IsTargetable = state;
            OnTargeted?.Invoke(IsTargetable);
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

        #region  Speed
        
        public void ApplySpeed(float speedPercentage, float duration = 0f, string effectId = null)
        {
            effectId ??= Guid.NewGuid().ToString();

            SpeedEffect effect = new SpeedEffect
            {
                speedMultiplier = 1 + speedPercentage,
                duration = duration,
                elapsedTime = 0f,
                isPermanent = duration <= 0f
            };

            activeEffects[effectId] = effect;
            UpdateMovementSpeed();
        }
        
        public void RemoveSpeed(string effectId)
        {
            if (activeEffects.Remove(effectId))
            {
                UpdateMovementSpeed();
            }
        }
        
        public void ClearTemporaryEffects()
        {
            List<string> keysToRemove = new List<string>();
            foreach (KeyValuePair<string, SpeedEffect> kvp in activeEffects)
            {
                if (!kvp.Value.isPermanent)
                    keysToRemove.Add(kvp.Key);
            }

            foreach (string key in keysToRemove)
                activeEffects.Remove(key);

            UpdateMovementSpeed();
        }
        
        private void Update()
        {
            UpdateSpeedEffects();
        }
        
        private void UpdateSpeedEffects()
        {
            List<string> expiredEffects = new List<string>();

            foreach ((string id, SpeedEffect effect) in activeEffects)
            {
                if (effect.isPermanent)
                    continue;

                SpeedEffect newEffect = new SpeedEffect(effect);
                newEffect.elapsedTime += Time.deltaTime;
                activeEffects[id] = newEffect;

                if (effect.elapsedTime >= effect.duration)
                {
                    expiredEffects.Add(id);
                }
            }

            // Remove expired effects
            foreach (string effectId in expiredEffects)
            {
                activeEffects.Remove(effectId);
            }

            if (expiredEffects.Count > 0)
            {
                UpdateMovementSpeed();
            }
        }

        private void UpdateMovementSpeed()
        {
            if (activeEffects.Count == 0)
            {
                pm.SetMovementSpeedMultiplier(1f);
                return;
            }

            // Combine all multipliers
            float totalMultiplier = 1f;
            foreach (SpeedEffect effect in activeEffects.Values)
            {
                totalMultiplier *= effect.speedMultiplier;
            }

            pm.SetMovementSpeedMultiplier(totalMultiplier);
        }

        #endregion
    }
}