using System;
using OverBang.GameName.Core;
using Sirenix.OdinInspector;
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
        [SerializeField] private TMP_InputField createSession;
        [SerializeField] private TMP_InputField joinSession;
        [SerializeField] private TMP_Dropdown maxPlayers;

        [SerializeField, ReadOnly] private bool isStarted;
        
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

        public async void JoinSession()
        {
            string raw = joinSession.text;
            string sessionId = raw.Trim();
            sessionId = sessionId.Replace(" ", string.Empty);
            joinSession.text = sessionId;
            
            if (sessionId == string.Empty)
                return;
            
            StartSession(sessionId);
        }

        public async void CreateSession()
        {
            string raw = createSession.text;
            string sessionId = string.IsNullOrWhiteSpace(raw) ? "DefaultSession" : raw.Trim();
            sessionId = sessionId.Replace(" ", string.Empty);
            createSession.text = sessionId;
            
            StartSession(sessionId);
        }
        
        private async void StartSession(string sessionID)
        {
            try
            {
                if (SessionManager.Global.IsAllowed)
                    return;
                
                SessionOptions options = new SessionOptions()
                {
                    Name = sessionID,
                    MaxPlayers = maxPlayers.value + 2,
                    IsPrivate = false,
                }.WithRelayNetwork();

                await SessionManager.Global.CreateOrJoinSession(sessionID, options);
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

                isStarted = false;

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

            Debug.Log("Starting game input");
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
            Debug.Log("Starting game RPC");
            SurvivalGameMode survivalGameMode = SurvivalGameMode.Create();
            survivalGameMode.SetGameMode();
        }
    }
}
