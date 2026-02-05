using System;
using OverBang.ExoWorld.Core.Utils;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
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

        private void OnHostGame(string serverName, int maxPlayers, ServerVisibility visibility)
        {
            SessionOptions options;
            if (visibility == ServerVisibility.Public)
            {
                options = new SessionOptions()
                {
                    Name = serverName,
                    MaxPlayers = maxPlayers,
                    IsPrivate = false,
                }.WithDistributedAuthorityNetwork();
            }
            else
            {
                options = new SessionOptions()
                {
                    Name = serverName,
                    MaxPlayers = maxPlayers,
                    IsPrivate = true,
                }.WithDistributedAuthorityNetwork();
            }
            
            CreateHostAsync(options);
        }

        private async void CreateHostAsync(SessionOptions gameOptions)
        {
            try
            {
                await SessionManager.Global.CreateSession(gameOptions);
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