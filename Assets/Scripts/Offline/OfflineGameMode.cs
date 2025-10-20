using OverBang.GameName.Hub;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Gameplay;
using UnityEngine;

namespace OverBang.GameName.Offline
{
    public class OfflineGameMode : IGameMode
    {
        public static OfflineGameMode Create(int map, int difficulty)
        {
            return new OfflineGameMode(map, difficulty);
        }

        public OfflineGameMode WithPlayer(params PlayerProfile[] profiles)
        {
            PlayerProfiles = profiles;
            return this;
        }

        public int Map { get; private set; }
        public int Difficulty { get; private set; }
        public PlayerProfile[] PlayerProfiles { get; private set; }
        public LevelManager LevelManager { get; private set; }

        private HubPhase.HubEndInfos hubEndInfos;
        private GameplayPhase.GameplayEndInfos gameplayEndInfos;

        private OfflineGameMode(int map, int difficulty)
        {
            Map = map;
            Difficulty = difficulty;
        }

        public void SetPlayerProfile(CharacterData character)
        {
            PlayerProfile profile = new PlayerProfile()
            {
                characterData = character,
                playerName = character.AgentName
            };
            SetPlayerProfile(profile);
        }
        
        public void SetPlayerProfile(params PlayerProfile[] profiles)
        {
            PlayerProfiles = profiles;
        }

        public async Awaitable Run()
        {
            bool isRunning = true;
            bool hasCharacter = PlayerProfiles != null && PlayerProfiles.Length != 0;

            while (isRunning)
            {
                HubPhase.SelectionSettings selectionSettings = new HubPhase.SelectionSettings
                {
                    selectionType = hasCharacter ? HubPhase.SelectionType.None : HubPhase.SelectionType.Pick,
                    availableClasses = CharacterClasses.All,
                    playerProfiles = hasCharacter ? PlayerProfiles : new PlayerProfile[]
                    {
                        new(null, "Player 1")
                    },
                    gameDatabase = GameController.GameDatabase,
                    localPlayer = 0
                };
                hubEndInfos = await HubPhase.CreateAsync(selectionSettings);
                
                SetPlayerProfile(hubEndInfos.selectedCharacters);
                hasCharacter = true;

                GameplayPhase.GameplaySettings gameplaySettings = new GameplayPhase.GameplaySettings
                {
                    gameDatabase = GameController.GameDatabase,
                    playerProfiles = PlayerProfiles,
                };
                gameplayEndInfos = await GameplayPhase.CreateAsync(gameplaySettings);
                
                if(gameplayEndInfos.isFinished)
                    isRunning = false;
            }
        }
    }
}