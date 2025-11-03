using System.Collections.Generic;
using Helteix.Tools;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Core.Menu
{
    public class LobbiesUI : MonoBehaviour
    {
        [SerializeField] private GameObject lobbyPrefab;
        [SerializeField] private Transform root;

        private List<LobbyUI> lobbies;
        private DynamicBuffer<LobbyUI> buffer;

        private void Awake()
        {
            lobbies = new List<LobbyUI>(4);
            buffer = new DynamicBuffer<LobbyUI>(4);
        }

        private void OnEnable()
        {
            UnityServices.Initialized += Register;
        }

        private void Register()
        {
            Refresh();
            MultiplayerService.Instance.SessionAdded += AddSession;
        }
        
        private void OnDisable()
        {
            MultiplayerService.Instance.SessionAdded -= AddSession;
            UnityServices.Initialized -= Register;
        }
        
        private void AddSession(ISession session)
        {
            GameObject lobbyInstance = Instantiate(lobbyPrefab, root);
            LobbyUI lobbyUI = lobbyInstance.GetComponent<LobbyUI>();
            
            if (lobbyUI != null)
            {
                lobbyUI.SetLobbyName(session.Name).SetPlayerCount(session.PlayerCount, session.MaxPlayers);
                lobbies.Add(lobbyUI);
            }
            else
            {
                Debug.LogError("LobbyEntryUI component not found on the instantiated lobby prefab.");
            }
        }

        public void Refresh()
        {
            buffer.CopyFrom(lobbies);
            
            for (int i = 0; i < buffer.Length; i++)
            {
                LobbyUI lobby = lobbies[i];
                lobby.Dispose();
                lobbies.Remove(lobby);
            }

            foreach ((string key, ISession value) in MultiplayerService.Instance.Sessions)
            {
                AddSession(value);
            }
        }
    }
}