namespace OverBang.GameName.Core.Characters
{
    [System.Flags]
    public enum CharacterClasses
    {
        None = 0,
        All = Attack | Defense | Support | Tactical,
        
        Attack = 1 << 0,
        Defense = 1 << 1,
        Support = 1 << 2,
        Tactical = 1 << 3
    }
}