using OverBang.GameName.Hub;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Gameplay;
using OverBang.GameName.Gameplay.Gameplay;
using UnityEngine;

namespace OverBang.GameName.Offline
{
    public class OfflineGameMode : IGameMode
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
        public LevelManager LevelManager { get; private set; }

        private OfflineGameMode(int map, int difficulty)
        {
            Map = map;
            Difficulty = difficulty;
            LevelManager = new LevelManager();
        }

        public void SetPlayerProfile(CharacterData character)
        {
            PlayerProfile profile = new PlayerProfile()
            {
                CharacterData = character,
                PlayerName = character.AgentName
            };
            SetPlayerProfile(profile);
        }
        
        public void SetPlayerProfile(PlayerProfile profile)
        {
            PlayerProfile = profile;
        }

        public async Awaitable Run()
        {
            if (!PlayerProfile.IsValid)
            {
                HubPhase.SelectionSettings selectionSettings = new HubPhase.SelectionSettings
                {
                    type = HubPhase.SelectionType.Pick,
                    availableClasses = CharacterClasses.All,
                    gameDatabase = GameController.GameDatabase
                };
                CharacterData newCharacter = await HubPhase.CreateAsync(selectionSettings);
                
                SetPlayerProfile(newCharacter);
            }
            
            //Handle GameplayPhase
            GameplayPhase.GameplayRewards rewards = await GameplayPhase.CreateAsync(new GameplayPhase.GameplaySettings
            {
                gameDatabase = GameController.GameDatabase,
                playerCharacter = PlayerProfile.CharacterData
            });
                
            Debug.LogError(rewards.score);
        }
    }
}