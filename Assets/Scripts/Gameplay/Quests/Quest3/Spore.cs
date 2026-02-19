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
        private QuestThreeHandler questThreeHandler;

        private readonly NetworkVariable<float> health = 
            new NetworkVariable<float>(readPerm: NetworkVariableReadPermission.Everyone, 
                writePerm: NetworkVariableWritePermission.Owner);

        private void Awake()
        {
            if (IsOwner)
                health.Value = questThreeData.SporeHealth;
            
            questThreeHandler ??= questThreeData.GetHandlerByData<QuestThreeHandler>();
            if (questThreeHandler == null)
            {
                gameObject.SetActive(false);
            }
        }

        public void TakeDamage(DamageData damage)
        {
            if (gameObject.activeSelf)
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