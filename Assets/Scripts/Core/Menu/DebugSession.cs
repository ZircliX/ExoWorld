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
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort) allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                );
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
            
            SessionOptions options = new SessionOptions()
            {
                Name = sessionId,
                MaxPlayers = 4,
                IsPrivate = false,
            };

            session = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionId, options);
            
            if (session.PlayerCount == 0)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }
            
            //NetworkManager.Singleton.SceneManager.LoadScene(GameMetrics.Global.SceneCollection.HubSceneRef.Name, LoadSceneMode.Single);
        }
    }
}