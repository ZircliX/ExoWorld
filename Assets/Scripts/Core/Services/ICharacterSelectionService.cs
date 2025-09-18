using System;
using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Core.Services
{
    public interface ICharacterSelectionService
    {
        void StartCharacterSelection(Action<CharacterData> onSelected);
    }
}