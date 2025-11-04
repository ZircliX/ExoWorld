using OverBang.GameName.Managers;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OverBang.GameName.Core.Menu
{
    public class LobbyUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text playerCountText;
        
        public ISessionInfo SessionInfo { get; private set; }
        
        public void Initialize(ISessionInfo sessionInfo)
        {
            SessionInfo = sessionInfo;
            nameText.text = SessionInfo.Name;

            int max = SessionInfo.MaxPlayers;
            int current = max - SessionInfo.AvailableSlots;
            playerCountText.text = $"{current}/{max} Players";
        }

        public void Dispose()
        {
            Destroy(gameObject);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _ = SessionManager.Global.JoinSessionByID(SessionInfo.Id);
        }
    }
}