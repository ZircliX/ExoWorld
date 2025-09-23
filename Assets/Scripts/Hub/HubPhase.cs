using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameAssets;
using OverBang.GameName.Core.Scene;
using OverBang.GameName.Managers;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace OverBang.GameName.Hub
{
    public class HubPhase
    {
        [System.Serializable]
        public enum SelectionType
        {
            Random,
            Pick
        }
        
        [System.Serializable]
        public struct SelectionSettings
        {
            public SelectionType type;
            public CharacterClasses availableClasses;
            public GameDatabase gameDatabase;
        }

        public static async Awaitable<CharacterData> CreateAsync(SelectionSettings settings)
        {
            SceneReference hubSceneRef = SceneCollection.Global.HubSceneRef;
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            if (currentSceneName != hubSceneRef.Path)
            {
                Task loadSceneAsync = SceneLoader.LoadSceneAsync(hubSceneRef.Name);
                await loadSceneAsync;
            }

            HubListener[] listeners = Object.FindObjectsByType<HubListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            HubPhase phase = new HubPhase(settings, listeners);

            await Awaitable.EndOfFrameAsync();
            
            await phase.Initialize();
            await AwaitableUtils.AwaitableUntil(() => phase.IsDone, CancellationToken.None);
            
            return phase.SelectedCharacter;
        }
        
        public event Action<bool> OnCompleted;
        public event Action<CharacterData> OnCharacterSelected;
        public event Action<CharacterData> OnAvailableCharacterAdded;

        private readonly SelectionSettings settings;
        private readonly HubListener[] listeners;
        
        public List<CharacterData> AvailableCharacters { get; private set; }
        public CharacterData SelectedCharacter { get; private set; }
        public bool IsDone { get; private set; }
        
        private HubPhase(SelectionSettings settings, HubListener[] listeners)
        {
            this.settings = settings;
            this.listeners = listeners;
        }

        private async Awaitable Initialize()
        {
            AvailableCharacters = new List<CharacterData>();
            
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].current = this;
                listeners[i].OnInit(this);
            }

            await settings.gameDatabase.ChangeCatalog(new DatabaseCatalog()
            {
                name = "Hub catalog",
            });

            //Debug.Log("HubPhase: Loading available characters...");
            AsyncOperationHandle operation = StartCharacterSelection();
            
            await operation.Task;
        }

        public AsyncOperationHandle StartCharacterSelection()
        {
            AsyncOperationHandle operation = settings.gameDatabase.LoadMultiple("AgentData", ctx =>
            {
                //Debug.Log("HubPhase: Processing loaded character " + ctx.name);
                if (ctx is not CharacterData characterData)
                    return;
                
                //Debug.Log($" Setting : {settings.availableClasses}, Character class: {characterData.CharacterClass}");
                if(!characterData.CharacterClass.Matches(settings.availableClasses))
                    return;
                    
                AvailableCharacters.Add(characterData);
                OnAvailableCharacterAdded?.Invoke(characterData);
            });
            return operation;
        }

        public void Complete(bool isSuccess)
        {
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].OnRelease(this);
            
            IsDone = true;
            OnCompleted?.Invoke(isSuccess);
        }

        public void SelectCharacter(CharacterData characterData)
        {
            SelectedCharacter = characterData;
            OnCharacterSelected?.Invoke(characterData);
        }
    }
}