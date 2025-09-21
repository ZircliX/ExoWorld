using System;
using System.Threading.Tasks;
using LTX.ChanneledProperties;
using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Core.Scene;
using OverBang.GameName.Core.Services;
using OverBang.GameName.Gameplay.Gameplay.StateMachine;
using OverBang.GameName.Managers;
using OverBang.GameName.Quests.QuestData;
using OverBang.GameName.Quests.QuestEvents;
using OverBang.GameName.Quests.QuestHandlers;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;
using ZTools.ObjectiveSystem.Core.Enum;
using ZTools.ObjectiveSystem.Core.Helpers;
using ZTools.ObjectiveSystem.Core.Interfaces;
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

        private void Initialize()
        {
            key = ChannelKey.GetUniqueChannelKey();
            
            GameController.CursorLockModePriority.AddPriority(key, PriorityTags.High);
            GameController.CursorVisibleStatePriority.AddPriority(key, PriorityTags.High);
            
            GameController.CursorLockModePriority.Write(key, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.Write(key, false);
            
            ObjectivesManager.OnGameEventDispatched += HandleGameEvent;
        }

        private void Dispose()
        {
            GameController.CursorLockModePriority.RemovePriority(key);
            GameController.CursorVisibleStatePriority.RemovePriority(key);
            
            ObjectivesManager.OnGameEventDispatched -= HandleGameEvent;
        }
        
        public async void Enter()
        {
            try
            {
                //Debug.Log("Gameplay started!");
            
                Initialize();
            
                if (SceneManager.GetActiveScene().name != "Map")
                {
                    await SceneLoader.LoadSceneAsync("Map");
                }
                
                await Task.Delay(100); // Wait a frame for everything to initialize
                
                spawnService.SpawnCharacter();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        public void Exit()
        {
            Dispose();
        }
        
        private void HandleGameEvent(IGameEvent gameEvent)
        {
            if (gameEvent is not ReachPointEvent reachPointEvent) return;
            
            if (reachPointEvent.PointID != "Extraction-Ship") return;
            
            stateMachine.ChangeState(new HubState(stateMachine, selectionService, spawnService));
        }
    }
}