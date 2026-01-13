using KBCore.Refs;
using TMPro;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.GameName.Core.Menus
{
    public class SessionCardUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private TMP_Text playerCountText;
        [SerializeField, Self] private Button button;
        
        public ISessionInfo SessionInfo { get; private set; }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

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

        public void OnClick()
        {
            Awaitable<ISession> aw = SessionManager.Global.JoinSessionByID(SessionInfo.Id);
            aw.Run();
        }
    }
}