using System;
using OverBang.ExoWorld.Core;
using Unity.Netcode;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay
{
    public class Pump : NetworkBehaviour, ITargetable, IDamageable
    {
        #region Quest

        [SerializeField] private PumpQuestData pumpQuestData;
        private PumpQuestHandler handler;

        public bool IsCompleted { get; private set; }
        public bool IsStarted { get; private set; }
        public float CurrentRepairTime { get; private set; }

        private const float REFRESH_RATE = 0.2f;
        private float couplingTimer;
        
        #endregion

        #region Interfaces

        public event Action<bool> OnTargetableChanged;
        public Transform Transform => transform;
        public TargetPriority Priority => TargetPriority.High;
        public bool IsTargetable => IsAlive && isTargetable;
        private bool isTargetable = true;
        public float Health { get; private set; }
        public float MaxHealth => pumpQuestData.TotalRepairHealth;
        public bool IsAlive => Health > 0;
        public bool IsInvincible { get; set; }
        public event Action OnDamaged;

        #endregion

        private void Awake()
        {
            Health = MaxHealth;
        }

        public void CallStartRepair()
        {
            CallStartRepairRpc(true);
            InvokePumpStartRpc();
        }

        [Rpc(SendTo.Owner)]
        private void CallStartRepairRpc(bool isStarted)
        {
            LevelManager.Instance.EnemySpawnerManager.SpawnEnemies(pumpQuestData.EnemySpawnScenario);
        }
        
        [Rpc(SendTo.Everyone)]
        private void InvokePumpStartRpc()
        {
            IsStarted = true;
            
            handler = pumpQuestData.GetHandlerByData<PumpQuestHandler>();
            handler.SetStepIndex(1);
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

        [Rpc(SendTo.Everyone)]
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
        
        public void SetTargetable(bool state)
        {
            isTargetable = state;
            OnTargetableChanged?.Invoke(state);
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
                IsStarted = false;
                IsCompleted = true;

                current = target;
                handler.SetStepIndex(2);
            }
            
            ObjectivesManager.DispatchGameEvent(new PumpEvent(current, target));
        }
    }
}