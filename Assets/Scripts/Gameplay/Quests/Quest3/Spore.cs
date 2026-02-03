using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Gameplay.Targeting;
using Unity.Netcode;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class Spore : NetworkBehaviour, IDamageable
    {
        [SerializeField] private QuestThreeData questThreeData;

        private readonly NetworkVariable<float> health = 
            new NetworkVariable<float>(readPerm: NetworkVariableReadPermission.Everyone, 
                writePerm: NetworkVariableWritePermission.Owner);

        private void Awake()
        {
            if (IsOwner)
                health.Value = questThreeData.SporeHealth;
        }

        public void TakeDamage(DamageData damage)
        {
            TakeDamageRpc(damage.baseDamage);
        }
        
        [Rpc(SendTo.Owner)]
        private void TakeDamageRpc(float finalDamage)
        {
            health.Value -= finalDamage;

            if (health.Value <= 0)
            {
                DispatchEventRpc();
                Destroy(gameObject);
            }
        }

        [Rpc(SendTo.Everyone)]
        private void DispatchEventRpc()
        {
            IGameEvent evt = new QuestThreeEvent();
            ObjectivesManager.DispatchGameEvent(evt);
        }
    }
}