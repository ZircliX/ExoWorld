using System.Collections.Generic;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class EnemySpawnerManager : PhaseListener<GameplayPhase>
    {
        private Dictionary<EnemyType, EnemyData> enemies;
        private HashSet<EnemyData> waveEnemies;
        
        public EnemySpawnScenario CurrentEnemySpawnScenario {get; private set;}

        
        
        protected override void OnBegin(GameplayPhase phase)
        {
            base.OnBegin(phase);

            enemies = new Dictionary<EnemyType, EnemyData>();
            
            EnemyData[] enemyRessources = Resources.LoadAll<EnemyData>("Enemies");
            for (int i = 0; i < enemyRessources.Length; i++)
            {
                enemies[enemyRessources[i].EnemyType] = enemyRessources[i];
            }
            
            
        }
        
        public void SpawnEnemies(EnemySpawnScenario enemySpawnScenario)
        {
            CurrentEnemySpawnScenario = enemySpawnScenario;
            HashSet<Area> spawnableZones = AreaManager.Instance.GetSpawnableAreas();
            switch (enemySpawnScenario.SpawnBehavior)
            {
                case EnemySpawnBehavior.SingleSpawn : 
                    foreach (Area area in spawnableZones)
                    {
                        foreach (var spawner in area.Spawners)
                        {
                            spawner.SpawnEnemies(
                                enemySpawnScenario.EnemyDatas[Random.Range(0, enemySpawnScenario.EnemyDatas.Length)], 
                                enemySpawnScenario.SpawnAmount
                            );
                        }
                    }
                    break;
                
                case EnemySpawnBehavior.MultipleSpawn :
                    break;
                
                case EnemySpawnBehavior.Wave :
                    Awaitable awaitable = StartWaveMode(enemySpawnScenario, spawnableZones);
                    awaitable.Run();
                    break;
            }
        }
        
        private async Awaitable StartWaveMode(EnemySpawnScenario enemySpawnScenario,  HashSet<Area> spawnableZones)
        {
            int currentWave = 0;
            int enemyToSpawnInWave = enemySpawnScenario.InitialEnemyAmountInWave; 

            //Wave mode loop
            while (currentWave <= enemySpawnScenario.WaveAmount)
            {
                enemyToSpawnInWave = enemySpawnScenario.GetEnemyAmountThisWave(enemyToSpawnInWave, enemySpawnScenario.EnemyAmountMultiplier);
                int currentSpawnedEnemies = 0;
                
                Debug.Log($"Wave {currentWave} started with {spawnableZones.Count} areas");

                //Single wave loop
                while (currentSpawnedEnemies <= enemyToSpawnInWave)
                {
                    foreach (Area area in spawnableZones)
                    {
                        int remainingEnemiesToSpawn = enemyToSpawnInWave - currentSpawnedEnemies;
                        currentSpawnedEnemies += await area.SpawnInSpawners(enemySpawnScenario, remainingEnemiesToSpawn, currentWave);
                        
                        if(currentSpawnedEnemies >= enemyToSpawnInWave)
                            break;
                    }
                }

                Debug.Log($"Wave{currentWave} spawned, waiting {enemySpawnScenario.TimeBetweenWaves} for the next one !");
                await Awaitable.WaitForSecondsAsync(enemySpawnScenario.TimeBetweenWaves);
                currentWave++;
            }
            
            Debug.Log("Wave Mode End");
        }
    }
}