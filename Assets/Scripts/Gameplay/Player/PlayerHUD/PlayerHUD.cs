using System.Collections.Generic;
using DG.Tweening;
using Helteix.Tools;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Player.PlayerHUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private Image healthBar;
        [SerializeField] private Image healthBarBg;
        
        [SerializeField, Space] private Transform teammateContainer;
        [SerializeField] private TeammateInfo teammatePrefab;
        private List<TeammateInfo> teammates;
        
        private LocalGamePlayer player;
        private LocalGamePlayer Player
        {
            get
            {
                player ??= GamePlayerManager.Instance.GetLocalPlayer();
                return player;
            }
        }

        private void Awake()
        {
            teammateContainer.ClearChildren();
            teammates = new List<TeammateInfo>(3);
        }

        private void OnEnable()
        {
            Player.OnHealthChanged += OnHealthChanged;
            
            RefreshPlayerStats();
            RefreshTeammates();
        }

        private void OnDisable()
        {
            Player.OnHealthChanged -= OnHealthChanged;
        }

        private void OnHealthChanged(float health, float maxHealth)
        {
            healthBar.fillAmount = health / maxHealth;
            healthBarBg.DOFillAmount(health / maxHealth, 0.2f);
        }

        private void RefreshPlayerStats()
        {
            if (Player.SessionPlayer.TryGetPlayerProperty(
                    GameMetrics.Global.ConstID.PlayerPropertyPlayerName,
                    out string playerName))
            {
                playerNameText.text = playerName;
            }
            else
            {
                playerNameText.text = $"Player_{Player.SessionPlayerID[..6]}";
            }
            
            OnHealthChanged(Player.Health, Player.MaxHealth);
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
                        teammateInfo.SetInfos(pName, remoteGamePlayer.CharacterData.Sprite);
                    }
                    
                    teammates.Add(teammateInfo);
                }
            }
        }
    }
}