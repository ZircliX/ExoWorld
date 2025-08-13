using System.Collections.Generic;
using KBCore.Refs;
using LTX.Singletons;
using OverBang.GameName.Player;

namespace OverBang.GameName.Managers
{
    public class PlayerManager : SceneSingleton<PlayerManager>
    {
        public List<PlayerController> Players { get; private set; }

        private void OnValidate() => this.ValidateRefs();

        public void RegisterPlayer(PlayerController playerController)
        {
            if (Players.Contains(playerController)) return;

            Players.Add(playerController);
        }

        public void UnregisterPlayer(PlayerController playerController)
        {
            if (!Players.Contains(playerController)) return;

            Players.Remove(playerController);
        }
    }
}