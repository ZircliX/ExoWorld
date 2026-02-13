using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Utils;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Characters
{
    public abstract class SelectionPhase : IPhase
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
            public SelectionType selectionType;
            public CharacterClasses availableClasses;

        }
        
        public readonly SelectionSettings Settings;
        
        public event Action<LocalGamePlayer, bool> OnCharacterSelected;
        
        public CharacterData SelectedCharacter { get; private set; }

        public bool IsDone { get; protected set; }
        
        public List<CharacterData> AvailableCharacters { get; protected set; }
        
        protected SelectionPhase(SelectionSettings selectionSettings)
        {
            Settings = selectionSettings;
            AvailableCharacters = new List<CharacterData>();
        }

        protected virtual async Awaitable OnBegin()
        {
            //Debug.Log("Starting Selection Phase");
            LoadCharactersData();
            
            await Task.CompletedTask;
        }

        protected virtual async Awaitable Execute()
        {
            await AwaitableUtils.AwaitableUntil(() => IsDone, CancellationToken.None);
        }
        
        protected virtual async Awaitable OnEnd()
        {
            await Task.CompletedTask;
        }

        public void SelectCharacter(CharacterData characterData, bool characterChanged)
        {
            if (characterData == null)
                return;
            
            //Debug.LogError($"SetPlayer property {ConstID.Global.PlayerPropertyCharacterData} for character {characterData.AgentName}");
            LocalGamePlayer localPlayer = GamePlayerManager.Instance.GetLocalPlayer();
            localPlayer.SetCharacterData(characterData);
            
            SelectedCharacter = characterData;
            OnCharacterSelected?.Invoke(localPlayer, characterChanged);
        }

        private void LoadCharactersData()
        {
            CharacterData[] characters = Resources.LoadAll<CharacterData>("Characters");
            CharacterClasses availableClasses = Settings.availableClasses;
            
            for (int i = 0; i < characters.Length; i++)
            {
                CharacterData character = characters[i];
                if (!character.CharacterClass.Matches(availableClasses))
                    continue;
                
                AvailableCharacters.Add(character);
            }
        }
        
        Awaitable IPhase.OnBegin() => OnBegin();
        Awaitable IPhase.OnEnd() => OnEnd();
        Awaitable IPhase.Execute() => Execute();
    }
}