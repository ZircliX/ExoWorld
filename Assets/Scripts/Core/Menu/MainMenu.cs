using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;

namespace OverBang.GameName.Network
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup playModeCanvas;
        [SerializeField] private CanvasGroup onlineCanvas;
        
        [SerializeField] private TMP_InputField inputField;
        
        [SerializeField] private UnityEvent onOfflineModeSelected;
        [SerializeField] private UnityEvent onOnlineModeSelected;
        
        public static string JoinCode;

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            
            AuthenticationService.Instance.SignedIn += OnSignedIn;
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        private void OnSignedIn()
        {
            Debug.Log($"Signed in : {AuthenticationService.Instance.PlayerId}");
        }

        public async void CreateRelay()
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
                
                JoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log($"Join code: {JoinCode}");
                //Debug.Log($"allocation.AllocationId: {allocation.AllocationId}");
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                    allocation.RelayServer.IpV4,
                    (ushort) allocation.RelayServer.Port,
                    allocation.AllocationIdBytes,
                    allocation.Key,
                    allocation.ConnectionData
                    );

                NetworkManager.Singleton.StartHost();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
            
            onOnlineModeSelected?.Invoke();
        }

        public async void JoinRelay()
        {
            try
            {
                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(inputField.text);
                Debug.Log($"Joined relay with allocation ID: {joinAllocation.AllocationId}");
                
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                    joinAllocation.RelayServer.IpV4,
                    (ushort) joinAllocation.RelayServer.Port,
                    joinAllocation.AllocationIdBytes,
                    joinAllocation.Key,
                    joinAllocation.ConnectionData,
                    joinAllocation.HostConnectionData
                    );
                
                NetworkManager.Singleton.StartClient();
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }

            onOnlineModeSelected?.Invoke();
        }

        public void PlayOfflineMode()
        {
            onOfflineModeSelected?.Invoke();
        }
    }
}