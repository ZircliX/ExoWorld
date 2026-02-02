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

        private float health;

        private void Awake()
        {
            health = questThreeData.SporeHealth;
        }

        public void TakeDamage(DamageData damage)
        {
            health -= damage.baseDamage;

            if (health <= 0)
            {
                IGameEvent evt = new QuestThreeEvent();
                ObjectivesManager.DispatchGameEvent(evt);
                Destroy(gameObject);
            }
        }
    }
}