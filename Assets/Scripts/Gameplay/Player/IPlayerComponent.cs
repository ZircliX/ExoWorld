using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Gameplay.Player
{
    public interface IPlayerComponent
    {
        PlayerController Controller { get; set; }
        void OnSync(CharacterData data);
    }
}