using DG.Tweening;
using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Managers;
using OverBang.GameName.Player;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace OverBang.GameName.Network
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mainCanvas;
        [SerializeField] private CanvasGroup playModeCanvas;
        [SerializeField] private CanvasGroup onlineCanvas;
        [SerializeField] private TMP_InputField inputField;
        
        [SerializeField] private PlayerController playerPrefab; 

        public static string JoinCode;

        private void Awake()
        {
            GameController.CursorLockModePriority.AddPriority(this, PriorityTags.High);
            GameController.CursorVisibleStatePriority.AddPriority(this, PriorityTags.High);
        }

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
                GameController.GameMode.SetGameMode(GameMode.GameModeType.Multiplayer);
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

            ChangeCanvasState(mainCanvas, false);
        }

        public async void JoinRelay()
        {
            try
            {
                GameController.GameMode.SetGameMode(GameMode.GameModeType.Multiplayer);
                
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
            
            ChangeCanvasState(mainCanvas, false);
        }
        
        public void StartSoloGame()
        {
            GameController.GameMode.SetGameMode(GameMode.GameModeType.Solo);
            PlayerController player = Instantiate(playerPrefab);
            ChangeCanvasState(mainCanvas, false);
        }

        public void OpenOnlineCanvas()
        {
            ChangeCanvasState(playModeCanvas, false);
            ChangeCanvasState(onlineCanvas, true);
        }

        public void OpenPlayModeCanvas()
        {
            ChangeCanvasState(onlineCanvas, false);
            ChangeCanvasState(playModeCanvas, true);
        }
        
        private void ChangeCanvasState(CanvasGroup canvasGroup, bool state)
        {
            canvasGroup.DOFade(state ? 1 : 0, 0.25f).OnComplete(() =>
            {
                canvasGroup.interactable = state;
                canvasGroup.blocksRaycasts = state;
            });

            if (canvasGroup == mainCanvas)
            {
                GameController.CursorLockModePriority.Write(this, CursorLockMode.Locked);
                GameController.CursorVisibleStatePriority.Write(this, false);
            }
        }
    }
}