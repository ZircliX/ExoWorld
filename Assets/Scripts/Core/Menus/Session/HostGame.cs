using System;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Core.Menus
{
    public class HostGame : MonoBehaviour
    {
        [SerializeField] private HostGameUI hostGameUI;

        private void Awake()
        {
            hostGameUI.OnCreateHostClicked += OnHostGame;
        }

        private void OnDestroy()
        {
            hostGameUI.OnCreateHostClicked -= OnHostGame;
        }

        private void OnHostGame(string serverName, int maxPlayers, ServerVisibility visibility, string password)
        {
            SessionOptions options = new SessionOptions()
            {
                Name = serverName,
                MaxPlayers = maxPlayers,
                IsPrivate = visibility != ServerVisibility.Public,
                Password = password
            }.WithRelayNetwork();
            
            CreateHostAsync(options);
        }

        private async void CreateHostAsync(SessionOptions gameOptions)
        {
            try
            {
                await SessionManager.Global.CreateSession(gameOptions);
                await Awaitable.WaitForSecondsAsync(0.2f);
                hostGameUI.OnHostCreated?.Invoke();
                //Debug.Log($"Host session {gameOptions.Name} created");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}