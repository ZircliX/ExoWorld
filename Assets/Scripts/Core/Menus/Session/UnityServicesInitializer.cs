using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
{
    public class UnityServicesInitializer : MonoBehaviour
    {
        private static bool isInitialized;

        private void Awake()
        {
            if (isInitialized)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            InitializeServices();
        }

        private async void InitializeServices()
        {
            try
            {
                await UnityServices.InitializeAsync();
                
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                    Debug.Log($"Signed in anonymously: {AuthenticationService.Instance.PlayerId}");
                }

                isInitialized = true;
                Debug.Log("Unity Services initialized successfully");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to initialize Unity Services: {ex.Message}");
                Destroy(gameObject);
            }
            
            // Clean up any session the Unity Services cloud thinks you're still in
            try
            {
                List<string> joinedSessions = await MultiplayerService.Instance.GetJoinedSessionIdsAsync();
                foreach (string sessionId in joinedSessions)
                {
                    ISession session = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionId);
                    await session.LeaveAsync();
                    Debug.LogWarning($"[Session] Left previous joined session: {sessionId}");
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SessionManager] Cleanup on init failed: {e.Message}");
            }

            // Then ensure NetworkManager is fully stopped
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.Shutdown();
                while (NetworkManager.Singleton.IsListening)
                    await Awaitable.NextFrameAsync();
            }
        }
    }
}