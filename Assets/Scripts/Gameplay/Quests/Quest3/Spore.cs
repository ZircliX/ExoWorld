using KBCore.Refs;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Gameplay.Targeting;
using Unity.Netcode;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class Spore : NetworkBehaviour, IDamageable
    {
        [SerializeField, Self] private NetworkObject networkObject;
        [SerializeField] private QuestThreeData questThreeData;
        
        [field: SerializeField] public Transform DamageTarget { get; private set; }
        private QuestThreeHandler questThreeHandler;

        private float health;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void Awake()
        {
            health = questThreeData.SporeHealth;
            
            questThreeHandler ??= questThreeData.GetHandlerByData<QuestThreeHandler>();
            if (questThreeHandler == null)
            {
                gameObject.SetActive(false);
            }
        }

        public void TakeDamage(RuntimeDamageData damage)
        {
            if (!gameObject.activeSelf)
                return;

            TakeDamageRpc(damage.finalDamage);
            
            if (health - damage.finalDamage < 0)
            {
                OwnerDestroyRpc();
            }
        }

        [Rpc(SendTo.Owner)]
        private void OwnerDestroyRpc()
        {
            networkObject.Despawn(false);
        }
        
        [Rpc(SendTo.Everyone)]
        private void TakeDamageRpc(float finalDamage)
        {
            health -= finalDamage;

            if (health <= 0)
            {
                if (questThreeHandler.StepIndex < 1)
                    questThreeHandler.SetStepIndex(1);
                
                DispatchEvent();
            }
        }

        private void DispatchEvent()
        {
            IGameEvent evt = new QuestThreeEvent();
            ObjectivesManager.DispatchGameEvent(evt);
            gameObject.SetActive(false);
        }
    }
}