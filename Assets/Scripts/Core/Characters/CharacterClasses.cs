namespace OverBang.ExoWorld.Core.Characters
{
    [System.Flags]
    public enum CharacterClasses
    {
        None = 0,
        All = -1,
        
        Attaque = 1 << 0,
        Défense = 1 << 1,
        Support = 1 << 2,
        Tactique = 1 << 3
    }
}