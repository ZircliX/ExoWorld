using Eflatun.SceneReference;
using OverBang.GameName.Core;
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

        public override async Awaitable OnBegin()
        {
            await base.OnBegin();
            
            SceneReference hubSceneRef = SceneCollection.Global.HubSceneRef;
            Scene currentScene = SceneLoader.GetCurrentScene();
    
            if (currentScene.name != hubSceneRef.Name && NetworkManager.Singleton.IsServer)
            {
                await SceneLoader.LoadSceneAsync(hubSceneRef.Name, LoadSceneMode.Single);
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