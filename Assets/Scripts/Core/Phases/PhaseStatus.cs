namespace OverBang.GameName.Core
{
    [System.Flags]
    [System.Serializable]
    public enum PhaseStatus : ushort
    {
        None = 0,
        SceneLoaded = 1 << 0,
        LevelLoaded = 1 << 1,
        PlayerSetup = 1 << 2,
        EnemiesSetup = 1 << 3,
        UISetup = 1 << 4,
        PoolsLoaded = 1 << 5,
    }
}