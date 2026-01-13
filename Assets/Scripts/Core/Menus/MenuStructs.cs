using UnityEngine;

namespace OverBang.GameName.Core.Menus
{
    [System.Serializable]
    public struct SelectorOption<T>
    {
        public string displayName;
        public T value;
        public Sprite icon;
    }

    public struct LobbyInfo
    {
        public string lobbyId;
        public string lobbyName;
        public int playerCount;
        public int maxPlayers;
    }

    public enum ServerVisibility
    {
        Public,
        Private,
        Friends
    }
}