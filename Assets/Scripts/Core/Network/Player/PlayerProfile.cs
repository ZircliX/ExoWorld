using OverBang.GameName.Core.Characters;

namespace OverBang.GameName.Offline.CharacterSelectionSystem
{
    [System.Serializable]
    public struct PlayerProfile
    {
        public string PlayerName;
        public CharacterData CharacterData;
        
        public bool IsValid => !string.IsNullOrEmpty(PlayerName) && CharacterData != null;
    }
}