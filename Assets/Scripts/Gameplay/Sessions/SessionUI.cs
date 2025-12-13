using System;
using System.Collections.Generic;
using OverBang.GameName.Core;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Sessions
{
    public class SessionUI : MonoBehaviour
    {
        [SerializeField] private Session session;
        [SerializeField] private SessionCardUI sessionCardPrefab;
        [SerializeField] private Transform root;
        
        [SerializeField] private GameObject startGameButton;
        [SerializeField] private GameObject quitSessionButton;
        [SerializeField] private GameObject waitingForHost;

        private List<SessionCardUI> lobbies;
        
        private float refreshTimer = 0f;
        private const float RefreshInterval = 2.5f;

        private void Awake()
        {
            refreshTimer = RefreshInterval;
            lobbies = new List<SessionCardUI>(4);
        }

        private void OnEnable()
        {
            SessionManager.Global.OnSessionChanged += OnSessionsChanged;
            
            if (UnityServices.Instance.State == ServicesInitializationState.Initialized)
            {
                OnServicesInitialized();
                session.OnJoinedSession += Refresh;
            }
            else
            {
                UnityServices.Initialized += SubToEvents;
            }
        }

        private void OnDisable()
        {
            SessionManager.Global.OnSessionChanged -= OnSessionsChanged;
            session.OnJoinedSession -= Refresh;
        }

        private void SubToEvents()
        {
            AuthenticationService.Instance.SignedIn += OnServicesInitialized;
        }

        private void OnServicesInitialized()
        {
            UnityServices.Initialized -= OnServicesInitialized;
            AuthenticationService.Instance.SignedIn -= OnServicesInitialized;
            
            Refresh();
        }
        
        private void OnSessionsChanged(ISession session)
        {
            if (session == null)
            {
                quitSessionButton.SetActive(false);
                startGameButton.SetActive(false);
                waitingForHost.SetActive(false);

                return;
            }
            
            quitSessionButton.SetActive(true);
            if (session.IsHost)
            {
                startGameButton.SetActive(true);
            }
            else
            {
                waitingForHost.SetActive(true);
            }
            
            Refresh();
        }

        private void Update()
        {
            if (SessionManager.Global.IsAllowed)
                return;
            
            refreshTimer -= Time.deltaTime;
            if (refreshTimer <= 0f)
            {
                Refresh();
            }
        }
        
        private void AddSession(ISessionInfo sessionInfo)
        {
            SessionCardUI sessionCardInstance = Instantiate(sessionCardPrefab, root);
            
            if (sessionCardInstance != null)
            {
                sessionCardInstance.Initialize(sessionInfo);
                lobbies.Add(sessionCardInstance);
            }
            else
            {
                Debug.LogError("LobbyEntryUI component not found on the instantiated lobby prefab.");
            }
        }

        public async void Refresh()
        {
            refreshTimer = RefreshInterval;
            
            try
            {
                // Query for available sessions
                QuerySessionsOptions queryOptions = new QuerySessionsOptions()
                {
                    Count = 5,
                    FilterOptions = new List<FilterOption>()
                    {
                        new FilterOption(FilterField.AvailableSlots, "1", FilterOperation.GreaterOrEqual)
                    }
                };

                IList<ISessionInfo> availableSessions = await SessionManager.Global.QuerySessions(queryOptions);
        
                //Debug.Log($"Found {availableSessions.Count} available sessions");
        
                for (int i = 0; i < availableSessions.Count; i++)
                {
                    ISessionInfo sessionInfo = availableSessions[i];
            
                    if (i < lobbies.Count)
                    {
                        // Reuse existing lobby UI
                        lobbies[i].Initialize(sessionInfo);
                    }
                    else
                    {
                        // Create new lobby UI if we don't have enough
                        AddSession(sessionInfo);
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