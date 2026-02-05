using System;
using OverBang.ExoWorld.Core.GameMode;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
{
    public class WaitingScreen : NetworkBehaviour
    {
        [SerializeField] private WaitingScreenUI waitingScreenUI;

        private void OnEnable()
        {
            waitingScreenUI.OnStartGameRpc += OnStartGameRpc;
        }

        private void OnDisable()
        {
            waitingScreenUI.OnStartGameRpc -= OnStartGameRpc;
        }

        private void OnStartGameRpc()
        {
            CreateGameModeRpc();
        }
        
        [Rpc(SendTo.Everyone)]
        private void CreateGameModeRpc()
        {
            if (GameModeUtils.TryGetGameModeForName(GameModeUtils.SurvivalGameModeName, out Type gameModeType))
            {
                object gameMode = Activator.CreateInstance(gameModeType);
                if (gameMode is IGameMode gameModeInstance)
                    gameModeInstance.SetGameMode();
                Debug.Log("create game mode RPC");
            }
        }
    }
}