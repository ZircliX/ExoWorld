using System;
using System.Collections.Generic;
using LTX.Singletons;
using OverBang.GameName.Player;

namespace OverBang.GameName.Managers
{
    public class PlayerManager : MonoSingleton<PlayerManager>
    {
        public List<PlayerController> Players { get; private set; }
        
        public event Action<string> OnPlayerRegistered;
        public event Action<string> OnPlayerUnregistered;

        protected override void Awake()
        {
            base.Awake();
            Players = new List<PlayerController>(4);
        }

        public void RegisterPlayer(PlayerController playerController)
        {
            if (Players.Contains(playerController)) return;

            Players.Add(playerController);
            OnPlayerRegistered?.Invoke(playerController.Guid);
        }

        public void UnregisterPlayer(PlayerController playerController)
        {
            if (!Players.Contains(playerController)) return;

            Players.Remove(playerController);
            OnPlayerUnregistered?.Invoke(playerController.Guid);
        }
    }
}