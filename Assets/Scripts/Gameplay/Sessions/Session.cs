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
        public ISession CurrentSession { get; private set; }

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
                if (UnityServices.Instance.State != ServicesInitializationState.Initialized)
                    return;
                
                SessionOptions options = new SessionOptions()
                {
                    Name = inputField.text == string.Empty ? "Default Session" : inputField.text,
                    MaxPlayers = 4,
                    IsPrivate = false,
                }.WithRelayNetwork();

                CurrentSession = await SessionManager.Global.CreateOrJoinSession(inputField.text, options);
                PlayerJoinedSessionRpc();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void StartGame()
        {
            if (!SessionManager.Global.IsHost())
                return;
            
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