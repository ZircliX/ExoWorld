using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
{
    [System.Serializable]
    public struct SelectorOption<T>
    {
        public string displayName;
        public T value;
        public Sprite icon;
    }

    public struct SessionInfo
    {
        public string sessionId;
        public string sessionName;
        public int currentPlayers;
        public int maxPlayers;
        public bool isPrivate;
    }

    public enum ServerVisibility
    {
        Public,
        Private,
        Friends
    }
}