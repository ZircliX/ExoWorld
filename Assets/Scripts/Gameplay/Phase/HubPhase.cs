using Eflatun.SceneReference;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Scene;
using OverBang.ExoWorld.Core.Utils;
using OverBang.Pooling;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace OverBang.ExoWorld.Gameplay.Phase
{
    public class HubPhase : SelectionPhase
    {
        public HubPhase(SelectionSettings selectionSettings) : base(selectionSettings)
        {
        }

        protected override async Awaitable OnBegin()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            await base.OnBegin();
            
            //Debug.Log("Starting Hub Phase");
            //Debug.Log("Updating player phase status to ReadyForSceneLoad");
            await SessionManager.Global.CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.ReadyForSceneLoad));
            await NetworkPropertiesUtils.AwaitableUntilAllPlayers(PhaseStatus.ReadyForSceneLoad);
            
            Debug.Log(SessionManager.Global.IsHost());
            if (SessionManager.Global.IsHost())
            {
                SceneReference hubSceneRef = SceneCollection.Global.HubSceneRef;
                Scene currentScene = SceneLoader.GetCurrentScene();
    
                if (currentScene.name != hubSceneRef.Name)
                {
                    await SceneLoader.LoadSceneAsync(hubSceneRef.Name, LoadSceneMode.Single);
                }
            }
            
            StartSelection();
        }

        protected override async Awaitable OnEnd()
        {
            await base.OnEnd();
            PoolManager.Instance.ClearPools();
            await SessionManager.Global.CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.None));
        }

        public void Validate()
        {
            IsDone = true;
        }

        private void StartSelection()
        {
            if (Settings.selectionType != SelectionType.None) return;

            LocalGamePlayer localPlayer = GamePlayerManager.Instance.GetLocalPlayer();
            SelectCharacter(localPlayer.CharacterData, false);

        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
        {
            if (scene.name == GameMetrics.Global.SceneCollection.HubSceneRef.Name)
            {
                Awaitable aw = SessionManager.Global.CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.SceneLoaded));
                aw.Run();
            }
        }
    }
}