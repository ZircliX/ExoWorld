using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.GameName.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        public void SpawnEnemies(EnemyData enemyData)
        {
            NetworkObject enemyObject = Object.Instantiate(GameMetrics.Global.EnemyPrefab, transform.position, transform.rotation);
            enemyObject.Spawn();
            
            enemyObject.TryGetComponent(out Enemy enemy);
            enemy.Initialize(enemyData);
        }

        public void DespawnEnemies(Enemy enemy)
        {
            enemy.networkObject.Despawn();
        }
    }
}