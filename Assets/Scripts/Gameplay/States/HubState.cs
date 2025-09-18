using System;
using LTX.ChanneledProperties;
using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Core;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Gameplay.States
{
    public class HubState : IGameState
    {
        private readonly Action<IGameState> stateTransition;

        public HubState(Action<IGameState> stateTransition)
        {
            this.stateTransition = stateTransition;
        }

        public void Enter()
        {
            ChannelKey key = ChannelKey.GetUniqueChannelKey();
            
            GameController.CursorLockModePriority.AddPriority(key, PriorityTags.High);
            GameController.CursorVisibleStatePriority.AddPriority(key, PriorityTags.High);
            
            GameController.CursorLockModePriority.Write(key, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.Write(key, false);
            
            // Subscribe to hub input (press "Start" to begin game)
            //HubController.OnGameStartRequested += HandleGameStart;
        }

        public void Exit()
        {
            //HubController.OnGameStartRequested -= HandleGameStart;
        }

        private void HandleGameStart()
        {
            stateTransition(new GameplayState(stateTransition));
        }
    }
}