using System.Collections.Generic;
using System.Linq;
using OverBang.ExoWorld.Core.Enemies;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Phase;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    public class EnemySpawnerManager : PhaseListener<GameplayPhase>
    {
        private struct WaveScenario
        {
            public EnemySpawnScenario scenario;
            public int currentWave;
            public bool isWaving;
        }
        private List<WaveScenario> waveModes;

        private Dictionary<EnemyType, EnemyData> enemies;
        private HashSet<EnemyData> waveEnemies;

        public EnemySpawnScenario CurrentEnemySpawnScenario { get; private set; }

        private HashSet<Area> previousSpawnableAreas;


        protected override void OnBegin(GameplayPhase phase)
        {
            base.OnBegin(phase);

            enemies = new Dictionary<EnemyType, EnemyData>();
            waveModes = new List<WaveScenario>();

            EnemyData[] enemyRessources = Resources.LoadAll<EnemyData>("Enemies");
            for (int i = 0; i < enemyRessources.Length; i++)
                enemies[enemyRessources[i].EnemyType] = enemyRessources[i];

            previousSpawnableAreas = AreaManager.Instance.Areas.ToHashSet();
        }

        protected override void OnEnd(GameplayPhase phase)
        {
            base.OnEnd(phase);

            // Arrête tous les wave modes actifs
            for (int i = 0; i < waveModes.Count; i++)
            {
                WaveScenario w = waveModes[i];
                w.isWaving = false;
                waveModes[i] = w;
            }
            waveModes.Clear();
        }

        public void StopWaveMode(EnemySpawnScenario scenario)
        {
            // Retrouve le WaveScenario correspondant et le stoppe
            for (int i = 0; i < waveModes.Count; i++)
            {
                if (waveModes[i].scenario == scenario)
                {
                    WaveScenario w = waveModes[i];
                    w.isWaving = false;
                    w.currentWave = scenario.WaveAmount; // force la fin de boucle
                    waveModes[i] = w;
                    return;
                }
            }
        }

        public void SpawnEnemies(EnemySpawnScenario enemySpawnScenario)
        {
            CurrentEnemySpawnScenario = enemySpawnScenario;

            switch (enemySpawnScenario.SpawnBehavior)
            {
                case EnemySpawnBehavior.SingleSpawn:
                    break;

                case EnemySpawnBehavior.MultipleSpawn:
                    break;

                case EnemySpawnBehavior.Wave:
                    // Crée un WaveScenario indépendant pour ce scenario
                    WaveScenario waveScenario = new WaveScenario
                    {
                        scenario = enemySpawnScenario,
                        currentWave = 1,
                        isWaving = true
                    };
                    // On récupère l'index AVANT de lancer l'async
                    int index = waveModes.Count;
                    waveModes.Add(waveScenario);

                    StartWaveMode(index).Run();
                    break;
            }
        }

        private async Awaitable StartWaveMode(int waveIndex)
        {
            EnemySpawnScenario enemySpawnScenario = waveModes[waveIndex].scenario;
            int enemyToSpawnInWave = enemySpawnScenario.InitialEnemyAmountInWave
                                     * SessionManager.Global.ActiveSession.PlayerCount / 2;

            while (waveModes[waveIndex].currentWave <= enemySpawnScenario.WaveAmount
                   && waveModes[waveIndex].isWaving)
            {
                enemyToSpawnInWave = enemySpawnScenario.GetEnemyAmountThisWave(
                    enemyToSpawnInWave,
                    enemySpawnScenario.EnemyAmountMultiplier
                );

                int currentSpawnedEnemies = 0;

                while (currentSpawnedEnemies < enemyToSpawnInWave
                       && waveModes[waveIndex].isWaving)
                {
                    HashSet<Area> spawnableAreas = AreaManager.Instance.GetSpawnableAreas();
                    if (spawnableAreas.Count == 0)
                        spawnableAreas = previousSpawnableAreas;

                    int spawnedThisFrame = 0;
                    foreach (Area area in spawnableAreas)
                    {
                        int remaining = enemyToSpawnInWave - currentSpawnedEnemies;
                        int spawned = await area.SpawnInSpawners(
                            enemySpawnScenario, remaining, waveModes[waveIndex].currentWave
                        );
                        currentSpawnedEnemies += spawned;
                        spawnedThisFrame += spawned;

                        if (currentSpawnedEnemies >= enemyToSpawnInWave)
                            break;
                    }

                    if (spawnableAreas.Count > 0)
                        previousSpawnableAreas = spawnableAreas;

                    // ✅ Si rien n'a pu spawner ce tour, on attend avant de réessayer
                    // Evite la boucle infinie si tous les spawners sont pleins/indisponibles
                    if (spawnedThisFrame == 0)
                        await Awaitable.WaitForSecondsAsync(0.5f);
                }

                await Awaitable.WaitForSecondsAsync(enemySpawnScenario.TimeBetweenWaves);

                WaveScenario w = waveModes[waveIndex];
                w.currentWave++;
                waveModes[waveIndex] = w;
            }

            WaveScenario done = waveModes[waveIndex];
            done.isWaving = false;
            waveModes[waveIndex] = done;

            Debug.Log($"Wave Mode End — Scenario index {waveIndex}");
        }
    }
}