using System;
using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Gameplay
{
    public class Pump : NetworkBehaviour, ITargetable, IDamageable
    {
        #region Quest

        [SerializeField] private PumpQuestData pumpQuestData;

        public event Action<float, float> OnProgressChanged;
        public bool IsCompleted { get; private set; }
        public bool IsStarted { get; private set; }
        public float CurrentRepairTime { get; private set; }

        private const float REFRESH_RATE = 0.2f;
        private float couplingTimer;
        
        #endregion

        #region Interfaces

        public Transform Transform => transform;
        public TargetPriority Priority => TargetPriority.High;
        public bool IsTargetable => IsAlive;
        public float Health { get; private set; }
        public float MaxHealth => pumpQuestData.TotalRepairHealth;
        public bool IsAlive => Health > 0;
        public event Action OnTargeted;
        public event Action OnDamaged;

        #endregion

        private void Start()
        {
            Health = MaxHealth;
        }

        public void CallStartRepair()
        {
            if (IsOwner)
            {
                SetIsStarted(true);
                LevelManager.Instance.EnemySpawnerManager.SpawnEnemies(pumpQuestData.EnemySpawnScenario);
            }
            else
            {
                CallStartRepairRpc(true);
            }
        }

        [Rpc(SendTo.Owner)]
        private void CallStartRepairRpc(bool isStarted)
        {
            SetIsStarted(isStarted);
        }

        private void SetIsStarted(bool isStarted)
        {
            if (!IsOwner) return;
            IsStarted = isStarted;
        }

        public void TakeDamage(DamageInfo damage)
        {
            if (IsOwner)
            {
                DamagePump();
            }
            else
            {
                CallHitPumpRpc();
            }
        }

        [Rpc(SendTo.Owner)]
        private void CallHitPumpRpc()
        {
            DamagePump();
        }

        private void DamagePump()
        {
            if (!IsOwner) return;
            Health--;
            OnDamaged?.Invoke();
        }

        public void Target()
        {
            OnTargeted?.Invoke();
        }

        private void Update()
        {
            if (!IsStarted || !IsHost) return;
            
            CurrentRepairTime += Time.deltaTime;
            couplingTimer += Time.deltaTime;

            if (couplingTimer >= REFRESH_RATE)
            {
                couplingTimer = 0;
                SendObjectiveProgressRpc(CurrentRepairTime, pumpQuestData.RepairTimeRequired);
            }
        }

        [Rpc(SendTo.Everyone)]
        private void SendObjectiveProgressRpc(float current, float target)
        {
            if (current >= target)
            {
                SetIsStarted(false);
                IsCompleted = true;

                current = target;
            }
            
            OnProgressChanged?.Invoke(current, target);
            ObjectivesManager.DispatchGameEvent(new PumpEvent(current, target));
        }
    }
}