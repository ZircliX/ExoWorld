using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Core.Menu
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text NameText;
        [SerializeField] private TMP_Text PlayerCountText;
        
        public ISessionInfo SessionInfo { get; private set; }
        
        public void Initialize(ISessionInfo sessionInfo)
        {
            SessionInfo = sessionInfo;
            NameText.text = SessionInfo.Name;

            int max = SessionInfo.MaxPlayers;
            int current = max - SessionInfo.AvailableSlots;
            PlayerCountText.text = $"{current}/{max} Players";
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }
    }
}