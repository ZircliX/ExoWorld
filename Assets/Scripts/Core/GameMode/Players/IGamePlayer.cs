using OverBang.ExoWorld.Core.Characters;

namespace OverBang.ExoWorld.Core.GameMode.Players
{
    public interface IGamePlayer
    {
        float Health { get; }
        float MaxHealth { get; }
        string SessionPlayerID { get; }
        ulong ClientID { get; }
        PlayerState State { get; }
        CharacterData CharacterData { get; }
    }
    
    public enum PlayerState { Uninitialized, Alive, Down, Dead }
}