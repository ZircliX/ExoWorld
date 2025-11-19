using System.Threading;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
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
                Debug.Log("Waiting");
                SessionManager.Global.CurrentPlayer.TryGetPhaseStatusByPlayer(out PhaseStatus status);
                return status == PhaseStatus.SceneLoaded;
            }, CancellationToken.None);
        }

        protected override async Awaitable<LevelManager> CreateLevelManager()
        {
            Debug.Log("Creating level manager");
            
            GameObject levelManager = new GameObject("LevelManager")
            {
                hideFlags = HideFlags.NotEditable
            };
            
            LevelManager = levelManager.AddComponent<LevelManager>();

            if (LevelManager != null)
            {
                await LevelManager.Initialize(this);
                LevelManager.StartLevel();
            }

            return LevelManager;
        }
    }
}