namespace OverBang.ExoWorld.Core
{
    [System.Flags]
    public enum CharacterClasses
    {
        None = 0,
        All = -1,
        
        Attack = 1 << 0,
        Defense = 1 << 1,
        Support = 1 << 2,
        Tactical = 1 << 3
    }
}