using System.Collections.Generic;
using Helteix.Singletons.MonoSingletons;

namespace OverBang.ExoWorld.Gameplay
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        private List<PlayerController> players;

        private void Awake()
        {
            players = new List<PlayerController>();
        }
        
        public void RegisterPlayer(PlayerController player)
        {
            players.Add(player);
        }

        public void UnregisterPlayer(PlayerController player)
        {
            players.Remove(player);
        }
    }
}