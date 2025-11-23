using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class Enemy : MonoBehaviour
    {
        [field : SerializeField] public NetworkObject networkObject;
        [field : SerializeField] public EnemyData enemyData;

        public void Initialize(EnemyData data)
        {
            enemyData = data;
        }

        public void OnMove()
        {
            
        }

        public void OnDeath()
        {
            
        }
        
        //LOGIQUE DE DEPLACEMENT ET TOUT ET TOUT.
        
    }
}