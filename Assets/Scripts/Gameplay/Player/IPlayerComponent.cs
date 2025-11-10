using OverBang.GameName.Core;

namespace OverBang.GameName.Gameplay
{
    public interface IPlayerComponent
    {
        PlayerController Controller { get; set; }
        void OnSync(CharacterData data);
    }
}