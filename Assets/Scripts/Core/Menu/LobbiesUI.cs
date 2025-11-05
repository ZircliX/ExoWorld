using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OverBang.GameName.Managers;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Core.Menu
{
    public class LobbiesUI : MonoBehaviour
    {
        [SerializeField] private LobbyUI lobbyPrefab;
        [SerializeField] private Transform root;

        private List<LobbyUI> lobbies;

        private void Awake()
        {
            lobbies = new List<LobbyUI>(4);
        }

        private void OnEnable()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
            {
                Refresh();
            }
            else
            {
                UnityServices.Initialized += OnServicesInitialized;
            }
        }

        private void OnServicesInitialized()
        {
            Refresh();
        }
    
        private void OnDisable()
        {
            UnityServices.Initialized -= OnServicesInitialized;
        }
        
        private async void Start()
        {
            while (this != null)
            {
                await Task.Delay(10000); // Refresh every 5 seconds
                if (this != null && gameObject.activeInHierarchy)
                {
                    Refresh();
                }
            }
        }
        
        private void AddSession(ISessionInfo session)
        {
            LobbyUI lobbyInstance = Instantiate(lobbyPrefab, root);
            
            if (lobbyInstance != null)
            {
                lobbyInstance.Initialize(session);
                lobbies.Add(lobbyInstance);
            }
            else
            {
                Debug.LogError("LobbyEntryUI component not found on the instantiated lobby prefab.");
            }
        }

        public async void Refresh()
        {
            try
            {
                // Query for available sessions
                QuerySessionsOptions queryOptions = new QuerySessionsOptions()
                {
                    Count = 20,
                };

                IList<ISessionInfo> availableSessions = await SessionManager.Global.QuerySessions(queryOptions);
        
                //Debug.Log($"Found {availableSessions.Count} available sessions");
        
                for (int i = 0; i < availableSessions.Count; i++)
                {
                    ISessionInfo session = availableSessions[i];
            
                    if (i < lobbies.Count)
                    {
                        // Reuse existing lobby UI
                        lobbies[i].Initialize(session);
                    }
                    else
                    {
                        // Create new lobby UI if we don't have enough
                        AddSession(session);
                    }
                }
        
                for (int i = availableSessions.Count; i < lobbies.Count; i++)
                {
                    lobbies[i]?.Dispose(); // Destroy the excess lobby UI
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to query sessions: {e}");
            }
        }
    }
}