using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Core
{
    public interface ICharacterSpawnService
    {
        void SpawnCharacter(CharacterData characterData);
    }
}