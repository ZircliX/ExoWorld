using System.Collections.Generic;
using OverBang.ExoWorld.Core.Utils;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private MainMenuUI mainMenuUI;
        [SerializeField] private HostGameUI hostGameUI;
        [SerializeField] private JoinGameUI joinGameUI;
        [SerializeField] private WaitingScreenUI waitingScreenUI;
        [SerializeField] private ContactsUI contactsUI;
        [SerializeField] private SettingsUI settingsUI;

        private IPanel currentPanel;
        private readonly Stack<IPanel> panelHistory = new Stack<IPanel>();

        private void Start()
        {
            InitializeListeners();
            
            ShowPanel(mainMenuUI);
            panelHistory.Push(mainMenuUI);
        }
        
        private void OnEnable()
        {
            NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
        }

        private void OnDisable()
        {
            if (NetworkManager.Singleton != null)
                NetworkManager.Singleton.OnTransportFailure -= OnTransportFailure;
        }

        private async void OnTransportFailure()
        {
            Debug.LogWarning("[SessionManager] Transport failure — recreating session...");
            await SessionManager.Global.LeaveCurrentSession();
    
            ShowPanel(mainMenuUI);
        }

        private void InitializeListeners()
        {
            mainMenuUI.OnHostClicked += () => ShowPanel(hostGameUI);
            mainMenuUI.OnJoinClicked += () => ShowPanel(joinGameUI);
            mainMenuUI.OnContactClicked += () => ShowPanel(contactsUI);
            mainMenuUI.OnSettingsClicked += () => ShowPanel(settingsUI);

            hostGameUI.OnBackClicked += GoBack;
            hostGameUI.OnHostCreated += () => ShowPanel(waitingScreenUI);

            joinGameUI.OnBackClicked += GoBack;
            joinGameUI.OnJoinedGame += () => ShowPanel(waitingScreenUI);
            
            waitingScreenUI.OnBackClicked += GoBack;
            
            contactsUI.OnBackClicked += GoBack;

            settingsUI.OnBackClicked += GoBack;
        }

        private void ShowPanel(IPanel panel)
        {
            if (currentPanel != null)
            {
                panelHistory.Push(currentPanel);
                currentPanel.Hide();
            }

            currentPanel = panel;
            currentPanel.Show();
        }

        private void GoBack()
        {
            if (panelHistory.Count <= 0)
                return;
            
            currentPanel.Hide();
            currentPanel = panelHistory.Pop();
            currentPanel.Show();
        }
    }
}