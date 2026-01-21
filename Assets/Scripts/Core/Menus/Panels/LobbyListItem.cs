using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core
{
    public class LobbyListItem : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private TMP_Text lobbyName;
        [SerializeField] private TMP_Text playerCount;
        [SerializeField] private Button selectButton;

        private SessionInfo sessionInfo;
        private Action<SessionInfo> onSelected;

        public void Initialize(SessionInfo session, Action<SessionInfo> callback)
        {
            sessionInfo = session;
            onSelected = callback;
            
            lobbyName.text = session.sessionName;
            playerCount.text = $"{session.currentPlayers}/{session.maxPlayers}";
            
            selectButton.onClick.AddListener(() => onSelected?.Invoke(sessionInfo));
        }

        public void OnSelect(BaseEventData eventData) => selectButton.onClick.Invoke();
        public void OnDeselect(BaseEventData eventData) { }
    }
}