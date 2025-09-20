using System;
using LTX.ChanneledProperties;
using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Core.Scene;
using OverBang.GameName.Core.Services;
using OverBang.GameName.Gameplay.Gameplay.StateMachine;
using OverBang.GameName.Managers;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace OverBang.GameName.Gameplay.States
{
    public class CharacterSelectionState : IGameState
    {
        public string Name => "Character Selection";
        
        private readonly ICharacterSelectionService selectionService;
        private readonly ICharacterSpawnService spawnService;
        private readonly StateMachine<IGameState> stateMachine;

        private ChannelKey key;

        public CharacterSelectionState(StateMachine<IGameState> stateMachine,
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
                key = ChannelKey.GetUniqueChannelKey();
            
                GameController.CursorLockModePriority.AddPriority(key, PriorityTags.High);
                GameController.CursorVisibleStatePriority.AddPriority(key, PriorityTags.High);
            
                GameController.CursorLockModePriority.Write(key, CursorLockMode.None);
                GameController.CursorVisibleStatePriority.Write(key, true);
            
                if (SceneManager.GetActiveScene().name != "Hub")
                {
                    await SceneLoader.LoadSceneAsync("Hub");
                }
            
                selectionService.StartCharacterSelection(OnCharacterSelected);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void OnCharacterSelected(CharacterData character)
        {
            selectionService.SetCharacter(character);
            spawnService.SpawnCharacter();
            stateMachine.ChangeState(new HubState(stateMachine, selectionService, spawnService));
        }

        public void Exit()
        {
            selectionService.StopCharacterSelection(OnCharacterSelected);

            GameController.CursorLockModePriority.RemovePriority(key);
            GameController.CursorVisibleStatePriority.RemovePriority(key);
        }
    }
}