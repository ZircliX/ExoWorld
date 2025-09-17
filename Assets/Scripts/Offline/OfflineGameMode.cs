using System;
using OverBang.GameName.CharacterSelection;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Scene;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace OverBang.GameName.Offline
{
    public class OfflineGameMode : IGameMode, ICharacterSelectionListener
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

        private OfflineGameMode(int map, int difficulty)
        {
            Map = map;
            Difficulty = difficulty;
        }
        
        public void SetPlayerProfile(PlayerProfile profile)
        {
            if (isGameRunning) return;
            
            PlayerProfile = profile;
            if (PlayerProfile.IsValid) LoadGameplayMap();
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
                
                    StartCharacterSelection();
                }
                else
                {
                    LoadGameplayMap();
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error activating offline mode: " + e);
            }
        }

        public void Deactivate()
        {
            Debug.LogError("DEACTIVATING OFFLINE GAME");
        }
        
        private void StartCharacterSelection()
        {
            CharacterSelectionManager.SelectionSettings settings = new CharacterSelectionManager.SelectionSettings
            {
                Type = CharacterSelectionManager.SelectionSettings.SelectionType.Pick,
                ClassLimitation = CharacterClasses.Tactical | CharacterClasses.Attack
            };
            
            CharacterSelectionManager.Instance.StartCharacterSelection(settings, HandleCharacterSelectionResult);
        }
        
        public void HandleCharacterSelectionResult(CharacterData character)
        {
            SetPlayerProfile(new PlayerProfile
            {
                PlayerName = character.AgentName,
                CharacterData = character
            });
            
            CharacterSelectionManager.Instance.OnCharacterSelected -= HandleCharacterSelectionResult;
        }

        private void LoadGameplayMap()
        {
            isGameRunning = true;
            Debug.LogError("STARTING OFFLINE GAME");
            SceneManager.LoadScene("Map");
        }
    }
}
