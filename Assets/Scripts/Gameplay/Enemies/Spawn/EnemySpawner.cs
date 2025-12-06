using OverBang.Pooling;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        public Enemy SpawnEnemy(EnemyData enemyData)
        {
            GameObject enemyGameObject = enemyData.EnemyResource.Spawn<GameObject>();

            if (enemyGameObject == null)
                return null;
            
            if (enemyGameObject.TryGetComponent(out NetworkObject networkObject))
            {
                enemyGameObject.transform.position = transform.position;
                networkObject.Spawn();
            }
            if (enemyGameObject.TryGetComponent(out Enemy enemy))
            {
                enemy.Initialize(enemyData.ID);
            }

            return enemy;
        }
    }
}