using System.Threading.Tasks;
using OverBang.GameName.Core;
using OverBang.GameName.Gameplay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Debugging
{
    [DefaultExecutionOrder(-10)]

public class DebugHubScene : MonoBehaviour
    {
        private async void Awake()
        {
            if (GameController.CurrentGameMode == null)
            {
                await LogIn();

                Debug.LogWarning($"No gamemode selected, Starting SurvivalGameMode from {nameof(DebugHubScene)}");
                SurvivalGameMode survivalGameMode = SurvivalGameMode.Create();
                survivalGameMode.SetGameMode();
            }
            else
            {
                Destroy(this);
            }
        }

        private async Task LogIn()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
                
            SessionOptions options = new SessionOptions()
            {
                Name = "Degub Session",
                MaxPlayers = 4,
                IsPrivate = false,
            }.WithRelayNetwork();

            await SessionManager.Global.CreateOrJoinSession("aaaaaaaaaa", options);
        }
    }
}