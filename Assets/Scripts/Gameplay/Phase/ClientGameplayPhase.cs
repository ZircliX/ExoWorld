using System.Threading;
using OverBang.ExoWorld.Core;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    public class ClientGameplayPhase : GameplayPhase
    {
        public ClientGameplayPhase(GameplaySettings gameplaySettings) : base(gameplaySettings)
        {
        }
        
        protected override async Awaitable LoadScene()
        {
            await AwaitableUtils.AwaitableUntil(() =>
            {
                //Debug.Log("Waiting");
                SessionManager.Global.CurrentPlayer.TryGetPhaseStatusByPlayer(out PhaseStatus status);
                return status == PhaseStatus.SceneLoaded;
            }, CancellationToken.None);
        }

        protected override LevelManager CreateLevelManager()
        {
            Debug.Log("Creating level manager");
            
            GameObject levelManager = new GameObject("LevelManager")
            {
                hideFlags = HideFlags.NotEditable
            };
            
            LevelManager = levelManager.AddComponent<LevelManager>();

            return LevelManager;
        }
    }
}