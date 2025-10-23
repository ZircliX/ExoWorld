using System;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Upgrades;

namespace OverBang.GameName.Core
{
    [System.Serializable]
    public struct PlayerProfile : IEquatable<PlayerProfile>
    {
        public string playerName;
        public CharacterData characterData;
        public UpgradeCollection upgradeCollection;

        public PlayerProfile(CharacterData characterData, string playerName, UpgradeCollection upgradeCollection)
        {
            this.characterData = characterData;
            this.playerName = playerName;
            this.upgradeCollection = upgradeCollection;
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