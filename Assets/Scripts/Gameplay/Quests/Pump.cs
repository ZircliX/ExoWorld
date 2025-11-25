using System;
using OverBang.GameName.Gameplay.QuestData;
using OverBang.GameName.Gameplay.QuestEvents;
using Unity.Netcode;
using UnityEngine;
using ZTools.ObjectiveSystem.Core.ZTools.ObjectiveSystem.Core;

namespace OverBang.GameName.Gameplay
{
    public class Pump : NetworkBehaviour
    {
        [SerializeField] private PumpQuestData pumpQuestData;

        public event Action<float, float> OnProgressChanged; 
        public bool IsCompleted { get; private set; }

        [field: SerializeField] public NetworkVariable<bool> IsStarted { get; private set; } = new NetworkVariable<bool>(
            readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);

        [field: SerializeField] public NetworkVariable<float> CurrentRepairTime { get; private set; } = new NetworkVariable<float>(
            readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<int> CurrentRepairHealth { get; private set; } = new NetworkVariable<int>(
            readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Owner);

        private const float REFRESH_RATE = 0.2f;
        private float couplingTimer;

        public void CallStartRepair()
        {
            if (IsOwner)
            {
                SetIsStarted(true);
                Debug.Log(LevelManager.Instance);
                Debug.Log(LevelManager.Instance.EnemySpawnerManager);
            }
            else
            {
                CallStartRepairRpc(true);
            }
            LevelManager.Instance.EnemySpawnerManager.SpawnEnemies(pumpQuestData.EnemySpawnScenario);
        }

        [Rpc(SendTo.Owner)]
        private void CallStartRepairRpc(bool isStarted)
        {
            SetIsStarted(isStarted);
        }

        private void SetIsStarted(bool isStarted)
        {
            if (!IsOwner) return;
            IsStarted.Value = isStarted;
        }

        public void CallHitPump()
        {
            if (IsOwner)
            {
                HitPump();
            }
            else
            {
                CallHitPumpRpc();
            }
        }

        [Rpc(SendTo.Owner)]
        private void CallHitPumpRpc()
        {
            HitPump();
        }

        private void HitPump()
        {
            if (!IsOwner) return;
            CurrentRepairHealth.Value--;
        }

        private void Update()
        {
            if (!IsStarted.Value || !IsHost) return;
            
            CurrentRepairTime.Value += Time.deltaTime;
            couplingTimer += Time.deltaTime;

            if (couplingTimer >= REFRESH_RATE)
            {
                couplingTimer = 0;
                SendObjectiveProgressRpc(CurrentRepairTime.Value, pumpQuestData.RepairTimeRequired);
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