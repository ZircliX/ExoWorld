using System;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Gameplay.Abilities;
using OverBang.ExoWorld.Gameplay.Level;
using OverBang.ExoWorld.Gameplay.Targeting;
using Unity.Netcode;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class Pump : NetworkBehaviour, ITargetable, IDamageable
    {
        [SerializeField] private DetectionArea enterDetectionArea;
        
        #region Quest

        [SerializeField] private QuestOneData questOneData;
        private QuestOneHandler oneHandler;

        public bool IsCompleted { get; private set; }
        public bool IsStarted { get; private set; }
        public float CurrentRepairTime { get; private set; }

        private const float REFRESH_RATE = 0.2f;
        private float couplingTimer;
        
        #endregion

        #region Interfaces

        public event Action<bool> OnTargeted;
        
        public TargetPriority Priority => TargetPriority.VeryHigh;
        public bool IsTargetable => IsAlive;
        public float Health { get; private set; }
        public float MaxHealth => questOneData.TotalRepairHealth;
        public bool IsAlive => Health > 0;
        public bool IsInvincible { get; private set; }
        public event Action OnDamaged;

        #endregion

        private void Awake()
        {
            Health = 0f;
            oneHandler ??= questOneData.GetHandlerByData<QuestOneHandler>();
            if (oneHandler == null)
            {
                gameObject.SetActive(false);
            }
            
            enterDetectionArea.SetAllowedTags("Player", "LocalPlayer");
        }

        private void OnEnable()
        {
            enterDetectionArea.OnEnter += OnEnter;
        }

        private void OnDisable()
        {
            enterDetectionArea.OnEnter -= OnEnter;
        }

        private void OnEnter(Collider arg1, object arg2)
        {
            if (oneHandler.StepIndex > 0) 
                return;
            
            OnEnterRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void OnEnterRpc()
        {
            enterDetectionArea.enabled = false;
            oneHandler.SetStepIndex(1);
        }

        public void CallStartRepair()
        {
            CallStartRepairRpc(true);
            InvokePumpStartRpc();
        }

        [Rpc(SendTo.Owner)]
        private void CallStartRepairRpc(bool isStarted)
        {
            LevelManager.Instance.EnemySpawnerManager.SpawnEnemies(questOneData.EnemySpawnScenario);
        }
        
        [Rpc(SendTo.Everyone)]
        private void InvokePumpStartRpc()
        {
            Health = questOneData.TotalRepairHealth;
            IsStarted = true;
            oneHandler.SetStepIndex(2);
        }

        public void TakeDamage(RuntimeDamageData damage)
        {
            if (IsOwner && Health > 0)
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
            Health--;
            Debug.Log("Pump health: " + Health);
            
            if (Health <= 0)
            {
                ResetPumpRpc();
                LevelManager.Instance.EnemySpawnerManager.StopWaveMode(questOneData.EnemySpawnScenario);
            }
            
            OnDamaged?.Invoke();
        }

        [Rpc(SendTo.Everyone)]
        private void ResetPumpRpc()
        {
            IsStarted = false;
            CurrentRepairTime = 0;
            ObjectivesManager.DispatchGameEvent(new QuestOneEvent(0, questOneData.RepairTimeRequired));
            oneHandler?.SetStepIndex(1);
        }
        
        public void SetTargetable(bool state)
        {
            OnTargeted?.Invoke(state);
        }

        private void Update()
        {
            if (!IsStarted || !IsHost) return;
            
            CurrentRepairTime += Time.deltaTime;
            couplingTimer += Time.deltaTime;

            if (couplingTimer >= REFRESH_RATE)
            {
                couplingTimer = 0;
                SendObjectiveProgressRpc(CurrentRepairTime, questOneData.RepairTimeRequired);
            }
        }

        [Rpc(SendTo.Everyone)]
        private void SendObjectiveProgressRpc(float current, float target)
        {
            if (current >= target)
            {
                IsStarted = false;
                IsCompleted = true;
                Health = 0;

                CurrentRepairTime = target;
            }
            
            ObjectivesManager.DispatchGameEvent(new QuestOneEvent(current, target));
        }
    }
}