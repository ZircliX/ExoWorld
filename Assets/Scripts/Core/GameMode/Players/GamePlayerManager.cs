using System.Collections.Generic;
using UnityEngine;

namespace OverBang.ExoWorld.Core.GameMode.Players
{
    public class GamePlayerManager : MonoBehaviour
    {
        [SerializeField] private float refreshRate = 0.3f;
        
        public static GamePlayerManager Instance { get; private set; }

        private float count;
        private Dictionary<string, IGamePlayer> players;
        public IReadOnlyCollection<IGamePlayer> Players => players.Values;
        
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
                if (gamePlayer.ClientID == clientId)
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
            foreach (IGamePlayer playersValue in players.Values)
            {
                if(playersValue is LocalGamePlayer player)
                    return player;
            }
            
            return null;
        }
    }
}