using System.Threading;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.CharacterSelection;
using OverBang.GameName.Core.Scenes;
using OverBang.GameName.Core.Utils;
using OverBang.GameName.Managers;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public class HubPhase : SelectionPhase
    {
        public HubPhase(SelectionSettings selectionSettings) : base(selectionSettings)
        {
        }

        public override async Awaitable OnBegin()
        {
            await base.OnBegin();
            
            SceneReference hubSceneRef = SceneCollection.Global.HubSceneRef;
            string currentSceneName = SceneLoader.GetCurrentSceneName();
            
            if (currentSceneName != hubSceneRef.Path)
            {
                bool loadingCompleted = false;
                
                void OnSceneEvent(SceneEvent sceneEvent)
                {
                    if (sceneEvent.SceneName == hubSceneRef.Name && sceneEvent.SceneEventType == SceneEventType.LoadComplete)
                    {
                        loadingCompleted = true;
                    }
                }

                NetworkManager.Singleton.SceneManager.OnSceneEvent += OnSceneEvent;
        
                try
                {
                    SceneLoader.NetworkLoadScene(hubSceneRef.Name);
                    await AwaitableUtils.AwaitableUntil(() => loadingCompleted == true, CancellationToken.None);
                }
                finally
                {
                    NetworkManager.Singleton.SceneManager.OnSceneEvent -= OnSceneEvent;
                }
            }
        }

        public override async Awaitable OnEnd(bool success)
        {
            await base.OnEnd(success);
        }

        public void StartSelection()
        {
            if (settings.selectionType == SelectionType.Pick)
            {
                StartCharacterSelection();
            }
            else
            {
                IPlayer currentPlayer = SessionManager.Global.CurrentPlayer;
                
                if (currentPlayer.TryGetCharacterDataByPlayer(out CharacterData character))
                {
                    SelectCharacter(character);
                }
            }
        }
    }
}