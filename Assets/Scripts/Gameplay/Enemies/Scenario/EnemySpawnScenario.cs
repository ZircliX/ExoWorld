using Sirenix.OdinInspector;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/Enemy/EnemySpawnScenario")]
    public class EnemySpawnScenario : ScriptableObject
    {
        [field : Header("Basic information")]
        [field : Space(10)]
        [field : SerializeField] public string Name {get; private set;}
        [field : SerializeField] public EnemySpawnBehavior SpawnBehavior {get; private set;}
        [field : SerializeField] public EnemyData[] EnemyDatas {get; private set;}
        [field : SerializeField] public int SpawnAmount {get; private set;}
        [field : SerializeField, MinMaxSlider(0f, 5f, ShowFields = true)] public Vector2 MinMaxSpawnIntervals { get; private set; }
        
        #region Wave
        
        [field : SerializeField, BoxGroup("WaveSettings", VisibleIf = "@SpawnBehavior == EnemySpawnBehavior.Wave")]
        public int WaveAmount {get; private set;}
        [field : SerializeField,  BoxGroup("WaveSettings", VisibleIf = "@SpawnBehavior == EnemySpawnBehavior.Wave")]
        public int InitialEnemyAmountInWave {get; private set;}
        [field : SerializeField,  BoxGroup("WaveSettings", VisibleIf = "@SpawnBehavior == EnemySpawnBehavior.Wave")] 
        public float EnemyAmountMultiplier {get; private set;}
        [field : SerializeField,  BoxGroup("WaveSettings", VisibleIf = "@SpawnBehavior == EnemySpawnBehavior.Wave" )] 
        public float TimeBetweenWaves {get; private set;}
        
        public int GetEnemyAmountThisWave(int initialAmount, float multiplier)
        {
            int Value = Mathf.CeilToInt(initialAmount * multiplier);
            return  Value;
        }
        
        #endregion Wave
        
    }
}