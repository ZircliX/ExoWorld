using System;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Core
{
    public class UnityServicesInitializer : MonoBehaviour
    {
        private static bool isInitialized = false;

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
        }
    }
}