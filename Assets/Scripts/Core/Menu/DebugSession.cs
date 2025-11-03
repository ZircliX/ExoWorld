using System.Collections.Generic;
using OverBang.GameName.Core.Metrics;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace OverBang.GameName.Core.Menu
{
    public class DebugSession : MonoBehaviour
    {
        [SerializeField] private string sessionId;
        private ISession session;

        private async void Awake()
        {
            await UnityServices.InitializeAsync();
            AuthenticationService.Instance.SignedIn += () => Debug.Log($"Signed in : {AuthenticationService.Instance.PlayerId}");
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void Update()
        {
            if (Keyboard.current.fKey.wasPressedThisFrame)
            {
                StartSession();
            }
            if (Keyboard.current.qKey.wasPressedThisFrame)
            {
                if (session == null) return;
                Debug.Log("Total Players : " + session.PlayerCount);
            }
        }

        private async void StartSession()
        {
            SessionOptions options = new SessionOptions()
            {
                Name = sessionId,
                MaxPlayers = 4,
                IsPrivate = false,
                Password = sessionId
            }.WithRelayNetwork();

            session = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionId, options);

            if (session.IsHost)
            {
                NetworkManager.Singleton.SceneManager.LoadScene(GameMetrics.Global.SceneCollection.HubSceneRef.Name, LoadSceneMode.Single);
            }
        }
    }
}