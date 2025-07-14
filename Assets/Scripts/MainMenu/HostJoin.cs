using DG.Tweening;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace TemaLeMultiLupeni.MainMenu
{
    public class HostJoin : MonoBehaviour
    {
        [SerializeField] private CanvasGroup menuCanvas;
        [SerializeField] private TMP_InputField inputField;
        
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
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(2);
                
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log($"Join code: {joinCode}");
                Debug.Log($"allocation.AllocationId: {allocation.AllocationId}");
                
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
            
            menuCanvas.DOFade(0, 0.25f);
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
            
            menuCanvas.DOFade(0, 0.25f);
        }
    }
}