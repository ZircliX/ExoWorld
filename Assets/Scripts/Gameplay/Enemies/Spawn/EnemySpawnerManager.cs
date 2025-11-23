using System.Collections.Generic;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class EnemySpawnerManager : PhaseListener<GameplayPhase>
    {
        private Dictionary<EnemyType, EnemyData> enemies;
        
        public EnemySpawnScenario CurrentEnemySpawnScenario {get; private set;}
        
        
        
        protected override void OnBegin(GameplayPhase phase)
        {
            base.OnBegin(phase);
            
            EnemyData[] enemyRessources = Resources.LoadAll<EnemyData>("Enemies");
            for (int i = 0; i < enemyRessources.Length; i++)
            {
                enemies[enemyRessources[i].EnemyType] = enemyRessources[i];
            }
            
        }

        protected override void OnEnd(GameplayPhase phase)
        {
            base.OnEnd(phase);
        }

        public void DebugEnemySpawn()
        {
            SpawnEnemies(CurrentEnemySpawnScenario, AreaManager.Instance.GetSpawnableAreas());
        }
        
        public void SpawnEnemies(EnemySpawnScenario enemySpawnScenario, HashSet<Area> spawnableZones)
        {
            //TODO spawns Schemas

            foreach (Area area in spawnableZones)
            {
                foreach (var spawner in area.Spawners)
                {
                    spawner.SpawnEnemies(enemySpawnScenario.EnemyDatas[Random.Range(0, enemySpawnScenario.EnemyDatas.Length)]);
                }
            }
            
        }
    }
}