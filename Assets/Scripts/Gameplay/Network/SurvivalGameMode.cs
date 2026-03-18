using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.GameMode;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Scene;
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
        private CancellationTokenSource cts;
        
        private GameplayPhase.GameplayEndInfos gameplayEndInfos;
        private bool hasCharacter;
        private bool isRunning;
        
        public IReadOnlyCollection<IGamePlayer> GamePlayers => gamePlayerManager.Players;
        private GamePlayerManager gamePlayerManager;
        private QuestManager questManager;

        public void End()
        {
            isRunning = false;
            cts?.Cancel();
        }
        
        public async Awaitable OnBegin()
        {
            cts = new CancellationTokenSource();
            
            gamePlayerManager = new GameObject(nameof(GamePlayerManager)).AddComponent<GamePlayerManager>();
            gamePlayerManager.hideFlags = HideFlags.NotEditable;
            
            Object.DontDestroyOnLoad(gamePlayerManager.gameObject);
            
            questManager = new QuestManager();
            
            await Task.CompletedTask;
        }

        public async Awaitable OnEnd()
        {
            cts?.Dispose();
            cts = null;
            
            if (gamePlayerManager != null)
                Object.Destroy(gamePlayerManager.gameObject);   
            
            await SceneLoader.LoadSceneAsync(GameMetrics.Global.SceneCollection.MainMenuSceneRef.Name);
            await SessionManager.Global.LeaveCurrentSession();
        }

        public async Awaitable Execute()
        {
            isRunning = true;
            hasCharacter = false;
            
            SetupGamePlayerManager();

            while (isRunning)
            {
                if (cts.IsCancellationRequested) break;
        
                questManager.RequestQuestQueue(1);
                CheckForCharacter();

                await HandleHubPhase();
                if (cts.IsCancellationRequested) break;

                GameplayPhase gameplayPhase = await HandleGameplayPhase();
                if (cts.IsCancellationRequested) break;
        
                gameplayEndInfos = gameplayPhase.CurrentEndInfos;
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
            
            HubPhase hubPhase = new HubPhase(selectionSettings, cts);
            await hubPhase.RunAsync();
        }
        
        private async Awaitable<GameplayPhase> HandleGameplayPhase()
        {
            GameplayPhase.GameplaySettings gameplaySettings = new GameplayPhase.GameplaySettings
            {
                
            };

            GameplayPhase gameplayPhase;
            if (SessionManager.Global.IsHost())
                gameplayPhase = new HostGameplayPhase(gameplaySettings, cts);
            else
                gameplayPhase = new ClientGameplayPhase(gameplaySettings, cts);
                
            await gameplayPhase.RunAsync();
            
            return gameplayPhase;
        }

        private void SetupGamePlayerManager()
        {
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
        }
    }
}