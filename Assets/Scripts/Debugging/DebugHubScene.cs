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
                Debug.LogWarning($"No gamemode selected, Starting SurvivalGameMode from {nameof(DebugHubScene)}");
                await LogIn();
                
                SurvivalGameMode survivalGameMode = SurvivalGameMode.Create();
                survivalGameMode.SetGameMode();
            }
            else
            {
                Destroy(this);
            }
        }

        private async Awaitable LogIn()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
                
            SessionOptions options = new SessionOptions()
            {
                Name = "Debug Hub Session",
                MaxPlayers = 2,
                IsPrivate = false,
            }.WithRelayNetwork();

            await SessionManager.Global.CreateOrJoinSession("DebugHubSession", options);
        }
    }
}