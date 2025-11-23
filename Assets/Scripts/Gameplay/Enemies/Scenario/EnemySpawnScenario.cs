using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/Enemy/EnemySpawnScenario")]
    public class EnemySpawnScenario : ScriptableObject
    {
        [field : SerializeField] public string Name {get; private set;}
        
        [field : SerializeField] public EnemySpawnBehavior EnemySpawnBehavior {get; private set;}
        
        [field : SerializeField] public EnemyData[] EnemyDatas {get; private set;}
        
        [field : SerializeField] public int SpawnAmount {get; private set;}
        [field : SerializeField] public float SpawnRate {get; private set;}
    }
}