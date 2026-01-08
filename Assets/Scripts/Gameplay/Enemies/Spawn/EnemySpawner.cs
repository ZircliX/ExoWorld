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

            NetworkObject instance = Instantiate(enemyData.EnemyPrefab,
                transform.position,
                Quaternion.identity);
            
            
            instance.SpawnWithOwnership(NetworkManager.Singleton.LocalClientId, true);

            if (instance == null)
                return null;

            if (instance.TryGetComponent(out Enemy enemy))
            {
                enemy.Initialize(enemyData.ID);
            }

            return enemy;
        }
    }
}