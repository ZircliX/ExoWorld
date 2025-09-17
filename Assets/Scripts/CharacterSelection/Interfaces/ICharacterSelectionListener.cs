using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.CharacterSelection
{
    public interface ICharacterSelectionListener
    {
        void HandleCharacterSelectionResult(CharacterData character);
    }
}