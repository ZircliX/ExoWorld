using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Online.Online.Sessions
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
                Debug.Log("Total Players" + session.PlayerCount);
            }
        }

        private async void StartSession()
        {
            SessionOptions options = new SessionOptions()
            {
                Name = sessionId,
                MaxPlayers = 4,
                IsPrivate = false,
            };

            session = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionId, options);
            Debug.Log("Session started with ID: " + session.Id);
        }
    }
}