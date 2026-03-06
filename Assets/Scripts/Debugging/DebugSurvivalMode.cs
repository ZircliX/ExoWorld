using OverBang.ExoWorld.Core;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Network;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace OverBang.ExoWorld.Debugging
{
    [DefaultExecutionOrder(-10)]
    public class DebugSurvivalMode : MonoBehaviour
    {
        [SerializeField] private string SessionName = "DebugHubSession";
        private async void Awake()
        {
            if (GameController.CurrentGameMode == null)
            {
                Debug.LogWarning($"No Gamemode selected, Starting SurvivalGameMode from {SceneManager.GetActiveScene().name}");
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
                Name = SessionName,
                MaxPlayers = 2,
                IsPrivate = false,
            }.WithRelayNetwork();

            await SessionManager.Global.CreateOrJoinSession(SessionName, options);
        }
    }
}