using OverBang.GameName.Hub;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.CharacterSelection;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Core.Metrics;
using OverBang.GameName.Core.Phases;
using OverBang.GameName.Gameplay;
using OverBang.GameName.Managers;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Offline
{
    public class SurvivalGameMode : IGameMode
    {
        public static SurvivalGameMode Create()
        {
            return new SurvivalGameMode();
        }

        private GameplayPhase.GameplayEndInfos gameplayEndInfos;

        private SurvivalGameMode()
        {
        }

        public async Awaitable Run()
        {
            bool isRunning = true;
            bool hasCharacter = false;
            
            while (isRunning)
            {
                hasCharacter = CheckForCharacter(hasCharacter);
                
                // Hub
                bool hubSuccess = await HandleHubPhase(hasCharacter);

                // Gameplay
                GameplayPhase gameplayPhase = await HandleGameplayPhase();

                if(gameplayPhase.CurrentEndInfos.isFinished)
                    isRunning = false;
            }
        }

        private static bool CheckForCharacter(bool hasCharacter)
        {
            if (SessionManager.Global.CurrentPlayer.TryGetPlayerProperty(
                    ConstID.Global.PlayerPropertyCharacterData, out string propertyValue))
            {
                hasCharacter = propertyValue != string.Empty;
                Debug.Log(hasCharacter);
            }

            return hasCharacter;
        }

        private static async Awaitable<bool> HandleHubPhase(bool hasCharacter)
        {
            SelectionPhase.SelectionSettings selectionSettings = new SelectionPhase.SelectionSettings
            {
                selectionType = hasCharacter ? SelectionPhase.SelectionType.None : SelectionPhase.SelectionType.Pick,
                availableClasses = CharacterClasses.All,
            };
                
            HubPhase hubPhase = new HubPhase(selectionSettings);
            bool hubSuccess = await hubPhase.Run();
            return hubSuccess;
        }
        
        private static async Awaitable<GameplayPhase> HandleGameplayPhase()
        {
            GameplayPhase.GameplaySettings gameplaySettings = new GameplayPhase.GameplaySettings
            {
            };

            GameplayPhase gameplayPhase;
            if (SessionManager.Global.IsHost)
            {
                gameplayPhase = new HostGameplayPhase(gameplaySettings);
            }
            else
            {
                gameplayPhase = new ClientGameplayPhase(gameplaySettings);
            }
                
            bool gameplaySuccess = await gameplayPhase.Run();
            return gameplayPhase;
        }

    }
}