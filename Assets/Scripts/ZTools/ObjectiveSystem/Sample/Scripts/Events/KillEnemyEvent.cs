using ZTools.ObjectiveSystem.Core.Interfaces;

namespace ZTools.ObjectiveSystem.Sample.Events
{
    /// <summary>
    /// Example concrete command for when an enemy is killed.
    /// </summary>
    //public readonly record struct KillEnemyEvent(string EnemyTag, int KillCount = 1) : IGameEvent;
    
        //You can also use a struct instead of a record struct if you prefer:
        
        public struct KillEnemyEvent : IGameEvent
        {
            public readonly string EnemyTag;
            public readonly int KillCount;

            public KillEnemyEvent(string enemyTag, int killCount = 1)
            {
                EnemyTag = enemyTag;
                KillCount = killCount;
            }
        }
}