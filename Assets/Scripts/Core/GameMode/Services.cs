using System;
using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Core
{
    public interface ICharacterSelectionService
    {
        void StartCharacterSelection(Action<CharacterData> onSelected);
    }

    public interface ICharacterSpawnService
    {
        void SpawnCharacter(CharacterData characterData);
    }

    public interface IGameStartService
    {
        void StartGame();
    }
}