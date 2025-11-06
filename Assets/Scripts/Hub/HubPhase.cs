using Eflatun.SceneReference;
using OverBang.GameName.Core.CharacterSelection;
using OverBang.GameName.Core.Scenes;
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
                await SceneLoader.LoadSceneAsync(hubSceneRef.Name);
            
            await Awaitable.NextFrameAsync();
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
        }
    }
}