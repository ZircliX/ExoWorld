using System;
using OverBang.GameName.CharacterSelection;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Core.Scene;
using OverBang.GameName.Core.Services;
using OverBang.GameName.Gameplay.States;
using UnityEngine;
using Object = UnityEngine.Object;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace OverBang.GameName.Offline
{
    public class OfflineGameMode : IGameMode, 
        ICharacterSelectionService, 
        ICharacterSpawnService, 
        IGameStartService
    {
        public static OfflineGameMode Create(int map, int difficulty)
        {
            return new OfflineGameMode(map, difficulty);
        }
        
        public OfflineGameMode WithPlayer(PlayerProfile profile)
        {
            PlayerProfile = profile;
            return this;
        }
        
        public int Map { get; private set; }
        public int Difficulty { get; private set; }
        public PlayerProfile PlayerProfile { get; private set; }

        private bool isGameRunning;
        private IGameState currentState;

        private OfflineGameMode(int map, int difficulty)
        {
            Map = map;
            Difficulty = difficulty;
        }
        
        public void SetPlayerProfile(PlayerProfile profile)
        {
            if (isGameRunning) return;
            
            PlayerProfile = profile;
        }
        
        public async void Activate()
        {
            try
            {
                if (!PlayerProfile.IsValid) //Start new offline session
                {
                    if (SceneManager.GetActiveScene().name != "Hub")
                    {
                        await SceneLoader.LoadSceneAsync("Hub");
                    }
                
                    TransitionTo(new SelectionState(this, this, TransitionTo));
                }
                else
                {
                    StartGame();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error activating offline mode: " + e);
            }
        }

        public void Deactivate()
        {
            currentState?.Exit();
            Debug.LogError("DEACTIVATING OFFLINE GAME");
        }
        
        public void TransitionTo(IGameState newState)
        {
            currentState?.Exit();
            currentState = newState;
            currentState.Enter();
        }
        
        public void StartCharacterSelection(Action<CharacterData> onSelected)
        {
            CharacterSelectionManager.Instance.StartCharacterSelection(
                new CharacterSelectionManager.SelectionSettings
                {
                    Type = CharacterSelectionManager.SelectionSettings.SelectionType.Pick,
                    ClassLimitation = CharacterClasses.None
                },
                onSelected);
        }

        public void SpawnCharacter(CharacterData characterData)
        {
            SetPlayerProfile(new PlayerProfile()
            {
                CharacterData = characterData,
                PlayerName = characterData.AgentName
            });
            Object.Instantiate(characterData.CharacterPrefab);
        }

        public async void StartGame()
        {
            try
            {
                if (SceneManager.GetActiveScene().name != "Map")
                {
                    await SceneLoader.LoadSceneAsync("Map");
                }

                SpawnCharacter(PlayerProfile.CharacterData);
                
                if (currentState is SelectionState)
                {
                    TransitionTo(new HubState(TransitionTo));
                }
                else
                {
                    TransitionTo(new GameplayState(TransitionTo));
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error starting offline game: " + e);
            }
        }
    }
}
