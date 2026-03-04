using OverBang.ExoWorld.Core.Enemies;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    public class EnemySpawner : MonoBehaviour
    {
        public Enemy SpawnEnemy(EnemyData enemyData)
        {
            NetworkSpawnManager spawnManager = NetworkManager.Singleton.SpawnManager;

            NetworkObject instance = spawnManager.InstantiateAndSpawn(
                enemyData.EnemyPrefab,
                NetworkManager.Singleton.LocalClientId,
                true,
                false,
                false,
                transform.position,
                Quaternion.identity);
            
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