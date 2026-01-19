using Eflatun.SceneReference;
using OverBang.GameName.Core;
using OverBang.Pooling;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

namespace OverBang.GameName.Hub
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
            //Debug.Log("Player phase status updated to ReadyForSceneLoad");
            
            //Debug.Log("Waiting for all players to be ready for scene load...");
            await NetworkPropertiesUtils.AwaitableUntilAllPlayers(PhaseStatus.ReadyForSceneLoad);
            
            if (SessionManager.Global.IsHost())
            {
                //Debug.Log("Load scene Hub for all players");
                SceneReference hubSceneRef = SceneCollection.Global.HubSceneRef;
                Scene currentScene = SceneLoader.GetCurrentScene();
    
                if (currentScene.name != hubSceneRef.Name && NetworkManager.Singleton.IsServer)
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
            if (Settings.selectionType == SelectionType.None)
            {
                IPlayer currentPlayer = SessionManager.Global.CurrentPlayer;
                
                if (currentPlayer.TryGetCharacterDataByPlayer(out CharacterData character))
                {
                    //Debug.Log($"PLayer {currentPlayer} got character {character.AgentName}");
                    SelectCharacter(character, false);
                }
            }
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