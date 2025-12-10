namespace OverBang.GameName.Core
{
    [System.Flags]
    [System.Serializable]
    public enum PhaseStatus : ushort
    {
        None = 0,
        ReadyForSceneLoad = 1 << 0,
        SceneLoaded = 1 << 1,
    }
}