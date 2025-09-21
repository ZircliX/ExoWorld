using System;
using System.Threading.Tasks;
using LTX.ChanneledProperties;
using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Core.Scene;
using OverBang.GameName.Core.Services;
using OverBang.GameName.Gameplay.Gameplay.StateMachine;
using OverBang.GameName.Managers;
using OverBang.GameName.Quests.QuestEvents;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;
using ZTools.ObjectiveSystem.Core.Interfaces;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace OverBang.GameName.Gameplay.States
{
    public class HubState : IGameState
    {
        public string Name => "Hub";
        
        private readonly ICharacterSelectionService selectionService;
        private readonly ICharacterSpawnService spawnService;
        private readonly StateMachine<IGameState> stateMachine;

        private ChannelKey key;

        public HubState(StateMachine<IGameState> stateMachine,
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
                Initialize();
                
                if (SceneManager.GetActiveScene().name != "Hub")
                {
                    await SceneLoader.LoadSceneAsync("Hub");
                }

                await Task.Delay(100);
                
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
            if (gameEvent.GetType() != typeof(HubStartEvent)) return;
            
            stateMachine.ChangeState(new GameplayState(stateMachine, selectionService, spawnService));
        }
    }
}