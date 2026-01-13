using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverBang.GameName.Core.Menus
{
    public class LobbyListItem : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private TMP_Text lobbyName;
        [SerializeField] private TMP_Text playerCount;
        [SerializeField] private Button selectButton;

        private LobbyInfo lobbyInfo;
        private Action<LobbyInfo> onSelected;

        public void Setup(LobbyInfo lobby, Action<LobbyInfo> callback)
        {
            lobbyInfo = lobby;
            onSelected = callback;
            
            lobbyName.text = lobby.lobbyName;
            playerCount.text = $"{lobby.playerCount}/{lobby.maxPlayers}";
            
            selectButton.onClick.AddListener(() => onSelected?.Invoke(lobbyInfo));
        }

        public void OnSelect(BaseEventData eventData) => selectButton.onClick.Invoke();
        public void OnDeselect(BaseEventData eventData) { }
    }
}