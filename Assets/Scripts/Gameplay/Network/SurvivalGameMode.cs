using System.Collections.Generic;
using System.Threading.Tasks;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.GameMode;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Phase;
using OverBang.ExoWorld.Gameplay.Quests;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Pool;

namespace OverBang.ExoWorld.Gameplay.Network
{
    [CreateGameMode(GameModeUtils.SurvivalGameModeName)]
    public class SurvivalGameMode : IGameMode
    {
        private GameplayPhase.GameplayEndInfos gameplayEndInfos;
        private bool hasCharacter;
        
        public IReadOnlyCollection<IGamePlayer> GamePlayers => gamePlayerManager.Players;
        
        private GamePlayerManager gamePlayerManager;
        
        private QuestManager questManager;
        
        public async Awaitable OnBegin()
        {
            gamePlayerManager = new GameObject(nameof(GamePlayerManager)).AddComponent<GamePlayerManager>();
            gamePlayerManager.hideFlags = HideFlags.NotEditable;
            
            Object.DontDestroyOnLoad(gamePlayerManager.gameObject);
            
            questManager = new QuestManager();
            
            await Task.CompletedTask;
        }

        public async Awaitable OnEnd()
        {
            Object.Destroy(gamePlayerManager.gameObject);
            await Task.CompletedTask;
        }

        public async Awaitable Execute()
        {
            //Debug.Log("Run GameMode: SurvivalGameMode");
            bool isRunning = true;
            hasCharacter = false;

            ISession session = SessionManager.Global.ActiveSession;
            IPlayer currentPlayer = SessionManager.Global.CurrentPlayer;
            using (ListPool<IGamePlayer>.Get(out List<IGamePlayer> gamePlayers))
            {
                foreach (IReadOnlyPlayer player in session.Players)
                {
                    //Local pour lois
                    if (player.Id == currentPlayer.Id)
                    {
                        LocalGamePlayer localGamePlayer = new LocalGamePlayer();
                        gamePlayers.Add(localGamePlayer);
                    }
                    else
                    {
                        RemoteGamePlayer remoteGamePlayer = new RemoteGamePlayer(player.Id);
                        gamePlayers.Add(remoteGamePlayer);
                    }
                }
                
                gamePlayerManager.Initialize(gamePlayers);
            }
            
            questManager.QueueNextQuest();

            while (isRunning)
            {
                CheckForCharacter();
                
                // Hub
                await HandleHubPhase();

                questManager.RequestQuestQueue();
                
                // Gameplay
                GameplayPhase gameplayPhase = await HandleGameplayPhase();
                gameplayEndInfos = gameplayPhase.CurrentEndInfos;

                if(gameplayPhase.CurrentEndInfos.isFinished)
                    isRunning = false;
            }
        }

        private void CheckForCharacter()
        {
            hasCharacter = gamePlayerManager.GetLocalPlayer().CharacterData != null;
        }

        private async Awaitable HandleHubPhase()
        {
            //Debug.Log("Creating Hub Phase");
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
                gameplayPhase = new HostGameplayPhase(gameplaySettings);
            else
                gameplayPhase = new ClientGameplayPhase(gameplaySettings);
                
            await gameplayPhase.RunAsync();
            
            return gameplayPhase;
        }
    }
}