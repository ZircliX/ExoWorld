using System;
using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Core
{
    public interface ICharacterSelectionService
    {
        void StartCharacterSelection(Action<CharacterData> onSelected);
    }
}