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

        private bool IsWaving; 

        
        
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

        protected override void OnEnd(GameplayPhase phase)
        {
            base.OnEnd(phase);
            IsWaving = false;
        }

        public void SpawnEnemies(EnemySpawnScenario enemySpawnScenario)
        {
            CurrentEnemySpawnScenario = enemySpawnScenario;
            HashSet<Area> spawnableZones = AreaManager.Instance.GetSpawnableAreas();
            switch (enemySpawnScenario.SpawnBehavior)
            {
                case EnemySpawnBehavior.SingleSpawn : 
                    break;
                
                case EnemySpawnBehavior.MultipleSpawn :
                    break;
                
                case EnemySpawnBehavior.Wave :
                    IsWaving = true;
                    Awaitable awaitable = StartWaveMode(enemySpawnScenario);
                    awaitable.Run();
                    break;
            }
        }
        
        private async Awaitable StartWaveMode(EnemySpawnScenario enemySpawnScenario)
        {
            int currentWave = 1;
            int enemyToSpawnInWave = enemySpawnScenario.InitialEnemyAmountInWave; 

            //Wave mode loop
            while (currentWave <= enemySpawnScenario.WaveAmount && IsWaving)
            {
                enemyToSpawnInWave = enemySpawnScenario.GetEnemyAmountThisWave(enemyToSpawnInWave, enemySpawnScenario.EnemyAmountMultiplier);
                int currentSpawnedEnemies = 0;

                HashSet<Area> spawnableZones = AreaManager.Instance.GetSpawnableAreas();
                Debug.Log($"Wave {currentWave} started with {spawnableZones.Count} areas");

                //Single wave loop
                while (currentSpawnedEnemies <= enemyToSpawnInWave)
                {
                    foreach (Area area in AreaManager.Instance.GetSpawnableAreas())
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
            IsWaving = false;
        }
    }
}