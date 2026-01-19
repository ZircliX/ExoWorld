using OverBang.GameName.Core;
using OverBang.GameName.Gameplay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Debugging
{
    [DefaultExecutionOrder(-10)]
    public class DebugMapScene : MonoBehaviour
    {
        private async void Awake()
        {
            if (GameController.CurrentGameMode == null)
            {
                Debug.LogWarning($"No gamemode selected, Starting SurvivalGameMode from {nameof(DebugMapScene)}");
                await LogIn();
                
                SurvivalGameMode survivalGameMode = new SurvivalGameMode();
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
                Name = "Debug Map Session",
                MaxPlayers = 2,
                IsPrivate = false,
            }.WithRelayNetwork();

            await SessionManager.Global.CreateOrJoinSession("DebugMapSession", options);
        }
    }
}
