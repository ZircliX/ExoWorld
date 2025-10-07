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
            
            GameObject go = new GameObject("LevelManager");
            LevelManager = go.AddComponent<LevelManager>();
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
            bool isRunning = true;

            while (isRunning)
            {
                HubPhase.SelectionSettings selectionSettings = new HubPhase.SelectionSettings
                {
                    type = HubPhase.SelectionType.Pick,
                    availableClasses = CharacterClasses.All,
                    preselectedCharacter = PlayerProfile.CharacterData,
                    gameDatabase = GameController.GameDatabase
                };
                CharacterData newCharacter = await HubPhase.CreateAsync(selectionSettings);
                SetPlayerProfile(newCharacter);

                GameplayPhase.GameplaySettings gameplaySettings = new GameplayPhase.GameplaySettings
                {
                    gameDatabase = GameController.GameDatabase,
                    playerCharacter = PlayerProfile.CharacterData
                };
                GameplayPhase.GameplayEndInfos endInfos = await GameplayPhase.CreateAsync(gameplaySettings);
            }
        }
    }
}