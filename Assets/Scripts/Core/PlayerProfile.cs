using System;
using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Core
{
    [System.Serializable]
    public struct PlayerProfile : IEquatable<PlayerProfile>
    {
        public string playerName;
        public CharacterData characterData;

        public PlayerProfile(CharacterData characterData, string playerName)
        {
            this.characterData = characterData;
            this.playerName = playerName;
        }

        public bool IsValid => !string.IsNullOrEmpty(playerName) && characterData != null;

        public bool Equals(PlayerProfile other)
        {
            return playerName == other.playerName && Equals(characterData, other.characterData);
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerProfile other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(playerName, characterData);
        }
    }
}