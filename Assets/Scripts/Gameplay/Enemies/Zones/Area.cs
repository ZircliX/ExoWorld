using System.Collections.Generic;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Enemies;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    [RequireComponent(typeof(Collider))]
    public class Area : MonoBehaviour
    {
        [Header("Refs")] 
        [SerializeField, Self] private Transform center;
        [SerializeField, Child] private EnemySpawner[] spawners;
        
        [SerializeField, ReadOnly] private int currentPlayersInArea;
        
        private List<Transform> players;
        private BoxCollider boxCollider;
        
        public bool IsValid => players.Count > 0;
        
        private void OnValidate()
        {
            this.ValidateRefs();
            
            if (players != null)
                currentPlayersInArea = players.Count;
        }

        private void Awake()
        {
            players = new List<Transform>();
        }

        private void Reset()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            this.ValidateRefs();
        }

        private void OnEnable()
        {
            AreaManager.Instance.Register(this);
        }

        private void OnDisable()
        {
            AreaManager.Instance.Unregister(this);
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("LocalPlayer"))
            {   
                players.Add(other.transform) ;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") || other.CompareTag("LocalPlayer"))
            {
                players.Remove(other.transform) ;
            }
        }

        public async Awaitable<int> SpawnInSpawners(EnemySpawnScenario enemySpawnScenario, int enemyToSpawn, int wave)
        {
            int spawnedEnemies = 0;
            for (int index = 0; index < spawners.Length; index++)
            {
                EnemySpawner spawner = spawners[index];
                EnemyData[] enemyDatas = enemySpawnScenario.EnemyDatas;

                int rnd = Random.Range(0, enemyDatas.Length);
                Enemy enemy = spawner.SpawnEnemy(enemyDatas[rnd]);
                if (enemy == null)
                    break;

                enemy.name = $"{enemy.EnemyData.EnemyName} | Area : {gameObject.name} | Spawner : {spawner.name}";
                enemy.transform.position = spawner.transform.position;
                spawnedEnemies++;

                await Awaitable.WaitForSecondsAsync(Random.Range(enemySpawnScenario.MinMaxSpawnIntervals.x,
                    enemySpawnScenario.MinMaxSpawnIntervals.y));
                if (spawnedEnemies >= enemyToSpawn)
                    break;
            }

            return spawnedEnemies;
        }
    }
}