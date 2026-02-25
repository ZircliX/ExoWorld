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

        private float health;

        private void Awake()
        {
            health = questThreeData.SporeHealth;
            
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
        
        [Rpc(SendTo.Everyone)]
        private void TakeDamageRpc(float finalDamage)
        {
            health -= finalDamage;

            if (health <= 0)
            {
                DispatchEvent();
                Destroy(gameObject);
            }
        }

        private void DispatchEvent()
        {
            IGameEvent evt = new QuestThreeEvent();
            ObjectivesManager.DispatchGameEvent(evt);
        }
    }
}