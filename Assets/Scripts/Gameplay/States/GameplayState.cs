using System;
using LTX.ChanneledProperties;
using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Core;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Gameplay.States
{
    public class GameplayState : IGameState
    {
        private readonly Action<IGameState> stateTransition;

        public GameplayState(Action<IGameState> stateTransition)
        {
            this.stateTransition = stateTransition;
        }
        
        public void Enter()
        {
            Debug.Log("Gameplay started!");
            
            ChannelKey key = ChannelKey.GetUniqueChannelKey();
            
            GameController.CursorLockModePriority.AddPriority(key, PriorityTags.High);
            GameController.CursorVisibleStatePriority.AddPriority(key, PriorityTags.High);
            
            GameController.CursorLockModePriority.Write(key, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.Write(key, false);
        }

        public void Exit() { }
    }
}