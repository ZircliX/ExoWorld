using OverBang.Pooling;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        public Enemy SpawnEnemy(EnemyData enemyData)
        {
            NetworkSpawnManager spawnManager = NetworkManager.Singleton.SpawnManager;
            NetworkObject enemyInstance = spawnManager.InstantiateAndSpawn(
                enemyData.EnemyPrefab,
                NetworkManager.Singleton.LocalClientId,
                true,
                false,
                false,
                transform.position);

            if (enemyInstance == null)
                return null;

            if (enemyInstance.TryGetComponent(out Enemy enemy))
            {
                enemy.Initialize(enemyData.ID);
            }

            return enemy;
        }
    }
}