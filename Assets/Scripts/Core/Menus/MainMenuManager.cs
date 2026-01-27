using System.Collections.Generic;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
{
    public class MainMenuManager : MonoBehaviour
    {
        [SerializeField] private MainMenuPanel mainMenuPanel;
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
            
            ShowPanel(mainMenuPanel);
            panelHistory.Push(mainMenuPanel);
        }

        private void InitializeListeners()
        {
            mainMenuPanel.OnHostClicked += () => ShowPanel(hostGameUI);
            mainMenuPanel.OnJoinClicked += () => ShowPanel(joinGameUI);
            mainMenuPanel.OnContactClicked += () => ShowPanel(contactsUI);
            mainMenuPanel.OnSettingsClicked += () => ShowPanel(settingsUI);

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