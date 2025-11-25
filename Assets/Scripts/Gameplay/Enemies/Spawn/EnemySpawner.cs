using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.GameName.Gameplay
{
    public class EnemySpawner : MonoBehaviour
    {
        public Enemy SpawnEnemy(EnemyData enemyData)
        {
            NetworkObject enemyObject = Object.Instantiate(GameMetrics.Global.EnemyPrefab, transform.position, transform.rotation);
            
            enemyObject.Spawn();

            if (enemyObject.TryGetComponent(out Enemy enemy))
            {
                enemy.Initialize(enemyData.ID);
            }

            return enemy;
        }
    }
}