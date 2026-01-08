using System;
using OverBang.GameName.Core;
using Sirenix.OdinInspector;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class Session : NetworkBehaviour
    {
        private enum SessionAction
        {
            Create,
            Join
        }
        
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

        public async void JoinSessionInput()
        {
            string raw = joinSession.text;
            string sessionId = raw.Trim();
            sessionId = sessionId.Replace(" ", string.Empty);
            joinSession.text = sessionId;
            
            if (sessionId == string.Empty)
                return;
            
            StartSession(SessionAction.Join, sessionId);
        }

        public async void CreateSessionInput()
        {
            string raw = createSession.text;
            string sessionId = string.IsNullOrWhiteSpace(raw) ? "DefaultSession" : raw.Trim();
            sessionId = sessionId.Replace(" ", string.Empty);
            createSession.text = sessionId;
            
            StartSession(SessionAction.Create, sessionId);
        }
        
        private async void StartSession(SessionAction action, string sessionID)
        {
            try
            {
                if (SessionManager.Global.IsAllowed)
                    return;

                if (MultiplayerService.Instance.Sessions.ContainsKey(sessionID))
                    action = SessionAction.Join;
                
                switch (action)
                {
                    case SessionAction.Create:
                        SessionOptions options = new SessionOptions()
                        {
                            Name = sessionID,
                            MaxPlayers = maxPlayers.value + 2,
                            IsPrivate = false,
                        }.WithRelayNetwork();
                        
                        await SessionManager.Global.CreateSession(options);
                        break;
                    
                    case SessionAction.Join:
                        await SessionManager.Global.JoinSessionByID(sessionID);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(action), action, null);
                }
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

        [Rpc(SendTo.Everyone)]
        private void PlayerJoinedSessionRpc()
        {
            OnJoinedSession?.Invoke();
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
        private void StartGameRpc()
        {
            Debug.Log("Starting game RPC");
            SurvivalGameMode survivalGameMode = SurvivalGameMode.Create();
            survivalGameMode.SetGameMode();
        }
    }
}
