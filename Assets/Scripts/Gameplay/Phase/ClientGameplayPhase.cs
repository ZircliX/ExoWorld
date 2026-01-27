using System.Threading;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Utils;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Phase
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
    }
}