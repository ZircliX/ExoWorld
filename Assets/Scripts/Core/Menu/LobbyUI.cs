using TMPro;
using UnityEngine;

namespace OverBang.GameName.Core.Menu
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text NameText;
        [SerializeField] private TMP_Text PlayerCountText;
        
        public LobbyUI SetLobbyName(string lobbyName)
        {
            NameText.text = lobbyName;
            return this;
        }
        
        public LobbyUI SetPlayerCount(int current, int max)
        {
            PlayerCountText.text = $"{current}/{max} Players";
            return this;
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}