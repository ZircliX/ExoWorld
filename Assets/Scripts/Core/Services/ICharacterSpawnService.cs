using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Core.Services
{
    public interface ICharacterSpawnService
    {
        void SpawnCharacter(CharacterData characterData);
    }
}