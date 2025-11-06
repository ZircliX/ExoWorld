using Codice.CM.Common.Tree;
using OverBang.GameName.Hub;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.CharacterSelection;
using OverBang.GameName.Core.GameMode;
using OverBang.GameName.Core.Metrics;
using OverBang.GameName.Core.Phases;
using OverBang.GameName.Gameplay;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Offline
{
    public class OfflineGameMode : IGameMode
    {
        public static OfflineGameMode Create(int map, int difficulty)
        {
            return new OfflineGameMode(map, difficulty);
        }

        public int Map { get; private set; }
        public int Difficulty { get; private set; }

        private GameplayPhase.GameplayEndInfos gameplayEndInfos;

        private OfflineGameMode(int map, int difficulty)
        {
            Map = map;
            Difficulty = difficulty;
        }

        public async Awaitable Run()
        {
            bool isRunning = true;
            bool hasCharacter = false;
            if (SessionManager.Global.CurrentPlayer.TryGetPlayerProperty(
                    ConstID.Global.PlayerPropertyCharacterData, out string propertyValue))
            {
                hasCharacter = propertyValue != string.Empty;
            }

            while (isRunning)
            {
                // Hub
                SelectionPhase.SelectionSettings selectionSettings = new SelectionPhase.SelectionSettings
                {
                    selectionType = hasCharacter ? SelectionPhase.SelectionType.None : SelectionPhase.SelectionType.Pick,
                    availableClasses = CharacterClasses.All,
                };
                
                HubPhase hubPhase = new HubPhase(selectionSettings);
                bool hubSuccess = await hubPhase.Run();
                hasCharacter = true;
                
                // Gameplay
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
                
                if(gameplayPhase.CurrentEndInfos.isFinished)
                    isRunning = false;
            }
        }
    }
}