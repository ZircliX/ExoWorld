using System;
using OverBang.GameName.Core;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Sessions
{
    public class Session : NetworkBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private TMP_Dropdown maxPlayers;

        private bool isStarted = false;
        
        public event Action OnJoinedSession;
        
        private async void Awake()
        {
            try
            {
                await UnityServices.InitializeAsync();
                AuthenticationService.Instance.SignedIn += () => Debug.Log($"Signed in : {AuthenticationService.Instance.PlayerId}");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public async void StartSession()
        {
            try
            {
                if (SessionManager.Global.IsAllowed)
                    return;
                
                string raw = inputField.text;
                string sessionId = string.IsNullOrWhiteSpace(raw) ? "DefaultSession" : raw.Trim();
                sessionId = sessionId.Replace(" ", string.Empty);
                inputField.text = sessionId;
                
                SessionOptions options = new SessionOptions()
                {
                    Name = sessionId,
                    MaxPlayers = maxPlayers.value + 2,
                    IsPrivate = false,
                }.WithRelayNetwork();

                await SessionManager.Global.CreateOrJoinSession(sessionId, options);
                PlayerJoinedSessionRpc();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        public async void QuitSession()
        {
            try
            {
                if (SessionManager.Global.IsAllowed)
                    return;

                await SessionManager.Global.LeaveCurrentSession();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void StartGame()
        {
            if (!SessionManager.Global.IsHost() || isStarted)
                return;

            isStarted = true;
            StartGameRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void PlayerJoinedSessionRpc()
        {
            OnJoinedSession?.Invoke();
        }

        [Rpc(SendTo.Everyone)]
        private void StartGameRpc()
        {
            SurvivalGameMode survivalGameMode = SurvivalGameMode.Create();
            survivalGameMode.SetGameMode();
        }
    }
}