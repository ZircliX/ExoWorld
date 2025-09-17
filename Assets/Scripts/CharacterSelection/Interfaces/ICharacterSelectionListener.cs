using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Offline.CharacterSelectionSystem.Interfaces
{
    public interface ICharacterSelectionListener
    {
        void HandleCharacterSelectionResult(CharacterData character);
    }
}