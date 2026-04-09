using System.Collections.Generic;
using DG.Tweening;
using Helteix.Tools;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Inventory;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Player.PlayerHUD
{
    public class PlayerHUD : MonoBehaviour, IPlayerComponent
    {
        [Header("Player Stats")]
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private Image playerIcon;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image healthBarBg;
        [SerializeField] private TMP_Text trinititeText;
        [SerializeField] private TMP_Text composantText;
        
        [Header("Teammates Stats")]
        [SerializeField, Space] private Transform teammateContainer;
        [SerializeField] private TeammateInfo teammatePrefab;
        private List<TeammateInfo> teammates;
        
        [Header("Other")]
        [SerializeField, Space] private GameObject questsPanel;
        [SerializeField] private GameObject timerPanel;
        
        [Header("Player")]
        [SerializeField] private CanvasGroup reviveCanvas;
        
        public PlayerController Controller { get; private set; }
        
        private void Awake()
        {
            teammateContainer.ClearChildren();
            teammates = new List<TeammateInfo>(3);

            bool isNeeded = SceneManager.GetActiveScene().name == GameMetrics.Global.SceneCollection.GameSceneRef.Name;
            timerPanel.SetActive(isNeeded);
            questsPanel.SetActive(isNeeded);
        }

        private void OnEnable()
        {
            RefreshPlayerStats();
            RefreshTeammates();
        }

        private void OnDestroy()
        {
            Controller.LocalGamePlayer.OnHealthChanged -= OnHealthChanged;
            Controller.LocalGamePlayer.Inventory.OnItemQuantityChanged -= OnItemQuantityChanged;
            Controller.LocalGamePlayer.OnStateChanged -= OnStateChanged;
        }

        private void OnStateChanged(PlayerState state)
        {
            reviveCanvas.DOFade(state == PlayerState.Down ? 1 : 0, 0.25f);
        }

        private void OnHealthChanged(float current, float maxHealth)
        {
            healthBar.fillAmount = current / maxHealth;
            healthBarBg.DOFillAmount(current / maxHealth, 0.5f);
        }
        
        private void OnItemQuantityChanged(ItemData itemData)
        {
            switch (itemData.ItemName)
            {
                case "Trinitite":
                    trinititeText.text = itemData.Quantity.ToString();
                    break;
                case "Composant":
                    composantText.text = itemData.Quantity.ToString();
                    break;
            }
        }

        private void RefreshPlayerStats()
        {
            if (Controller.LocalGamePlayer.SessionPlayer.TryGetPlayerProperty(
                    GameMetrics.Global.ConstID.PlayerPropertyPlayerName,
                    out string playerName))
            {
                playerNameText.text = playerName;
            }
            else
            {
                playerNameText.text = $"Player_{Controller.LocalGamePlayer.SessionPlayerID[..6]}";
            }
            
            playerIcon.sprite = Controller.LocalGamePlayer.CharacterData.Sprite;
        }

        private void RefreshTeammates()
        {
            teammates.Clear();
            teammateContainer.ClearChildren();

            foreach (IGamePlayer gamePlayer in GamePlayerManager.Instance.Players)
            {
                if (gamePlayer is RemoteGamePlayer remoteGamePlayer)
                {
                    TeammateInfo teammateInfo = Instantiate(teammatePrefab, teammateContainer);
                    
                    if (remoteGamePlayer.SessionPlayer.TryGetPlayerProperty(
                            GameMetrics.Global.ConstID.PlayerPropertyPlayerName, out string pName))
                    {
                        teammateInfo.SetInfos(pName);
                    }
                    
                    teammates.Add(teammateInfo);
                }
            }
        }
        
        public void OnSync(PlayerRuntimeContext context)
        {
            Controller = context.playerController;
            Controller.LocalGamePlayer.OnHealthChanged += OnHealthChanged;
            Controller.LocalGamePlayer.Inventory.OnItemQuantityChanged += OnItemQuantityChanged;
            Controller.LocalGamePlayer.OnStateChanged += OnStateChanged;
        }
    }
}