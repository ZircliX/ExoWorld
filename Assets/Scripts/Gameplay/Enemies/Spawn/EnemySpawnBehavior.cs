namespace OverBang.ExoWorld.Gameplay.Enemies
{ 
    public enum EnemySpawnBehavior
    {
        Wave, //Multiple spawns with rounds pattern.
        SingleSpawn, //Spawn one time a specific quantity.
        MultipleSpawn, //Spawn multiple enemies at sequenced timings.
    }
}