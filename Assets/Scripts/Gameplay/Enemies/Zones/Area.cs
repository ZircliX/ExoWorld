using System.Collections.Generic;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    [RequireComponent(typeof(BoxCollider))]
    public class Area : MonoBehaviour
    {
        [Header("Bounds Parameters")]
        [field: SerializeField, Self, HideInInspector] public Transform Center { get; private set; }
        
        [field: SerializeField, Child] public List<EnemySpawner> Spawners { get; private set; }
        private List<Transform> players;
        private BoxCollider boxCollider;

        private void OnValidate()
        {
            this.ValidateRefs();
            
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
        
        public bool IsValid()
        {
            return players.Count > 0;
        }

        public async Awaitable<int> SpawnInSpawners(EnemySpawnScenario enemySpawnScenario, int enemyToSpawn, int wave)
        {
            int spawnedEnemies = 0;
            foreach (EnemySpawner spawner in Spawners)
            {
                EnemyData[] enemyDatas = enemySpawnScenario.EnemyDatas;
                
                int rnd = Random.Range(0, enemyDatas.Length);
                Enemy enemy = spawner.SpawnEnemy(enemyDatas[rnd]);
                if (enemy == null)
                    break;
                
                enemy.name = $"{enemy.enemyData.name} | Wave {wave} | Area : {gameObject.name} & Spawner : {spawner.name}| EnemyLeft : {enemyToSpawn - spawnedEnemies} | ";
                enemy.transform.position = spawner.transform.position;
                spawnedEnemies++;

                await Awaitable.WaitForSecondsAsync(Random.Range(enemySpawnScenario.MinMaxSpawnIntervals.x, enemySpawnScenario.MinMaxSpawnIntervals.y));
                if (spawnedEnemies >= enemyToSpawn)
                    break;
            }

            return spawnedEnemies;
        }
    }
}