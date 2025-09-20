using System;
using LTX.ChanneledProperties;
using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Core.Scene;
using OverBang.GameName.Core.Services;
using OverBang.GameName.Gameplay.Gameplay.StateMachine;
using OverBang.GameName.Managers;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace OverBang.GameName.Gameplay.States
{
    public class GameplayState : IGameState
    {
        public string Name => "Gameplay";
        
        private readonly ICharacterSelectionService selectionService;
        private readonly ICharacterSpawnService spawnService;
        private readonly StateMachine<IGameState> stateMachine;
        
        private ChannelKey key;

        public GameplayState(StateMachine<IGameState> stateMachine,
            ICharacterSelectionService selectionService,
            ICharacterSpawnService spawnService)
        {
            this.stateMachine = stateMachine;
            this.selectionService = selectionService;
            this.spawnService = spawnService;
        }
        
        public async void Enter()
        {
            try
            {
                Debug.Log("Gameplay started!");
            
                key = ChannelKey.GetUniqueChannelKey();
            
                GameController.CursorLockModePriority.AddPriority(key, PriorityTags.High);
                GameController.CursorVisibleStatePriority.AddPriority(key, PriorityTags.High);
            
                GameController.CursorLockModePriority.Write(key, CursorLockMode.Locked);
                GameController.CursorVisibleStatePriority.Write(key, false);
            
                if (SceneManager.GetActiveScene().name != "Map")
                {
                    await SceneLoader.LoadSceneAsync("Map");
                }
                
                spawnService.SpawnCharacter();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Exit()
        {
            GameController.CursorLockModePriority.RemovePriority(key);
            GameController.CursorVisibleStatePriority.RemovePriority(key);
        }
    }
}