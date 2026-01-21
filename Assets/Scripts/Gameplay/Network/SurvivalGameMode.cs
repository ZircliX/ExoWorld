using System.Threading.Tasks;
using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    [CreateGameMode(GameModeUtils.SurvivalGameModeName)]
    public class SurvivalGameMode : IGameMode
    {
        private GameplayPhase.GameplayEndInfos gameplayEndInfos;
        private bool hasCharacter;

        public SurvivalGameMode()
        {
        }
        
        public async Awaitable OnBegin()
        {
            await Task.CompletedTask;
        }

        public async Awaitable OnEnd()
        {
            await Task.CompletedTask;
        }

        public async Awaitable Execute()
        {
            Debug.Log("Run GameMode: SurvivalGameMode");
            bool isRunning = true;
            hasCharacter = false;
            
            while (isRunning)
            {
                CheckForCharacter();
                
                // Hub
                await HandleHubPhase();

                // Gameplay
                GameplayPhase gameplayPhase = await HandleGameplayPhase();
                gameplayEndInfos = gameplayPhase.CurrentEndInfos;

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
            Debug.Log("Creating Hub Phase");
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
            if (SessionManager.Global.IsHost())
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