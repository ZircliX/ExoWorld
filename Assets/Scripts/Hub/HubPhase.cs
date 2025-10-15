using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eflatun.SceneReference;
using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.GameAssets;
using OverBang.GameName.Core.Scenes;
using OverBang.GameName.Managers;
using OverBang.Pooling;
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
            None,
            Random,
            Pick,
        }
        
        [System.Serializable]
        public struct SelectionSettings
        {
            public int PlayerCount => playerProfiles?.Length ?? 1;
            public SelectionType selectionType;
            public CharacterClasses availableClasses;
            public int localPlayer;
            
            public PlayerProfile[] playerProfiles;
            public GameDatabase gameDatabase;
        }
        
        [System.Serializable]
        public struct HubEndInfos
        {
            public PlayerProfile[] selectedCharacters;
        }

        public static async Awaitable<HubEndInfos> CreateAsync(SelectionSettings settings)
        {
            SceneReference hubSceneRef = SceneCollection.Global.HubSceneRef;
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            if (currentSceneName != hubSceneRef.Path)
                await SceneLoader.LoadSceneAsync(hubSceneRef.Name, setActive: true);

            HubListener[] listeners = Object.FindObjectsByType<HubListener>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            HubPhase phase = new HubPhase(settings, listeners);

            await Awaitable.EndOfFrameAsync();
            
            await phase.Initialize();
            await AwaitableUtils.AwaitableUntil(() => phase.IsDone, CancellationToken.None);
            
            HubEndInfos endInfos = new HubEndInfos()
            {
                selectedCharacters = phase.PlayerProfiles
            };
            
            return endInfos;
        }
        
        public event Action<bool> OnCompleted;
        public event Action<int, PlayerProfile> OnCharacterSelected;
        public event Action<CharacterData> OnAvailableCharacterAdded;

        public readonly SelectionSettings Settings;
        private readonly HubListener[] listeners;
        
        public List<CharacterData> AvailableCharacters { get; private set; }
        public PlayerProfile[] PlayerProfiles { get; private set; }
        public bool IsDone { get; private set; }
        
        private HubPhase(SelectionSettings settings, HubListener[] listeners)
        {
            this.Settings = settings;
            this.listeners = listeners;
        }

        private async Awaitable Initialize()
        {
            AvailableCharacters = new List<CharacterData>();
            
            await Settings.gameDatabase.ChangeCatalog(new DatabaseCatalog()
            {
                name = "Hub catalog",
                assetsKeys = new List<object>(),
                labels = new List<string>()
            });
            
            for (int i = 0; i < listeners.Length; i++)
            {
                listeners[i].current = this;
                listeners[i].OnInit(this);
            }

            PlayerProfiles = Settings.playerProfiles;
            
            if (Settings.selectionType != SelectionType.None)
            {
                AsyncOperationHandle operation = StartCharacterSelection();
                await operation.Task;
            }
            else
            {
                OnCharacterSelected?.Invoke(Settings.localPlayer, PlayerProfiles[Settings.localPlayer]);
            }
        }

        public AsyncOperationHandle StartCharacterSelection()
        {
            AsyncOperationHandle operation = Settings.gameDatabase.LoadMultiple("AgentData", ctx =>
            {
                //Debug.Log("HubPhase: Processing loaded character " + ctx.name);
                if (ctx is not CharacterData characterData)
                    return;
                
                //Debug.Log($" Setting : {settings.availableClasses}, Character class: {characterData.CharacterClass}");
                if(!characterData.CharacterClass.Matches(Settings.availableClasses))
                    return;
                    
                AvailableCharacters.Add(characterData);
                OnAvailableCharacterAdded?.Invoke(characterData);
            });
            return operation;
        }

        public void CompletePhase(bool isSuccess)
        {
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].OnRelease(this);
            
            IsDone = true;
            OnCompleted?.Invoke(isSuccess);
        }

        public void SelectLocalCharacter(CharacterData playerProfile) =>
            SelectCharacter(Settings.localPlayer, playerProfile);
        
        public void SelectCharacter(int index, CharacterData playerProfile)
        {
            PlayerProfile profile = new PlayerProfile()
            {
                playerName = PlayerProfiles[index].playerName,
                characterData = playerProfile
            };
            PlayerProfiles[index] = profile;
            
            OnCharacterSelected?.Invoke(index, profile);
        }
    }
}