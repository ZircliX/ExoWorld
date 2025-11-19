using OverBang.GameName.Core;
using OverBang.GameName.Hub;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class SurvivalGameMode : IGameMode
    {
        public static SurvivalGameMode Create()
        {
            return new SurvivalGameMode();
        }

        private GameplayPhase.GameplayEndInfos gameplayEndInfos;
        private bool hasCharacter;

        private SurvivalGameMode()
        {
        }

        public async Awaitable Run()
        {
            bool isRunning = true;
            hasCharacter = false;
            
            while (isRunning)
            {
                CheckForCharacter();
                
                // Hub
                await HandleHubPhase();

                Debug.Log("Between Hub and Gameplay");
                
                // Gameplay
                GameplayPhase gameplayPhase = await HandleGameplayPhase();

                if(gameplayPhase.CurrentEndInfos.isFinished)
                    isRunning = false;
            }
        }

        private void CheckForCharacter()
        {
            if (SessionManager.Global.CurrentPlayer.TryGetPlayerProperty(
                    ConstID.Global.PlayerPropertyCharacterData, out string propertyValue))
            {
                hasCharacter = propertyValue != string.Empty;
            }
        }

        private async Awaitable HandleHubPhase()
        {
            SelectionPhase.SelectionSettings selectionSettings = new SelectionPhase.SelectionSettings
            {
                selectionType = hasCharacter ? SelectionPhase.SelectionType.None : SelectionPhase.SelectionType.Pick,
                availableClasses = CharacterClasses.All,
            };
                
            HubPhase hubPhase = new HubPhase(selectionSettings);
            await hubPhase.RunAsync();
        }
        
        private async Awaitable<GameplayPhase> HandleGameplayPhase()
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
                
            await gameplayPhase.RunAsync();
            
            return gameplayPhase;
        }

    }
}