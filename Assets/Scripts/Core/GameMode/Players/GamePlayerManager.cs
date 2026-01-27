using System;
using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;
using UnityEngine;

namespace OverBang.ExoWorld.Core.GameMode.Players
{
    public class GamePlayerManager : MonoBehaviour
    {
        public static GamePlayerManager Instance { get; private set; }

        private Dictionary<string, IGamePlayer> players;
        
        public IReadOnlyCollection<IGamePlayer> Players => players.Values;
        

        [SerializeField]
        private float refreshRate = .3f;
        
        private float count;
        
        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
            
        }
        
        private void LateUpdate()
        {
            count += Time.deltaTime;
            if (count >= refreshRate)
            {
                count = 0;
                LocalGamePlayer local = GetLocalPlayer();
                local?.ApplyIfDirty();
            }

        }

        public bool TryGetPlayerWithClientId(ulong clientId, out IGamePlayer player)
        {
            foreach ((string id, IGamePlayer gamePlayer) in players)
            {
                if(gamePlayer.ClientID == clientId)
                {
                    player = gamePlayer;
                    return true;
                }
            }
            player = null;
            return false;
        }
        
        public bool TryGetPlayerWithSessionId(string sessionID, out IGamePlayer player) => players.TryGetValue(sessionID, out player);
        
        public void Initialize(List<IGamePlayer> newPlayers)
        {
            players = new Dictionary<string, IGamePlayer>(4);
            foreach (IGamePlayer player in newPlayers)
                players.Add(player.SessionPlayerID, player);
        }

        public LocalGamePlayer GetLocalPlayer()
        {
            foreach (var playersValue in players.Values)
            {
                if(playersValue is LocalGamePlayer player)
                    return player;
            }
            
            return null;
        }
    }
}