using System;
using System.Collections.Generic;
using System.Threading;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Core
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
        
        public event Action<IPlayer, CharacterData, bool> OnCharacterSelected;
        
        public CharacterData SelectedCharacter { get; private set; }
        public IPlayer CurrentPlayer => SessionManager.Global.CurrentPlayer;

        public bool IsDone { get; protected set; }
        
        public List<CharacterData> AvailableCharacters { get; protected set; }
        
        protected SelectionPhase(SelectionSettings selectionSettings)
        {
            Settings = selectionSettings;
            AvailableCharacters = new List<CharacterData>();
        }

        protected virtual async Awaitable OnBegin()
        {
            CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.None));
            
            /* Not sure if it's useful and prob creates bugs
            if (!CurrentPlayer.Properties.ContainsKey(ConstID.Global.PlayerPropertyCharacterData))
            {
                CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyCharacterData,
                    new PlayerProperty(string.Empty));
            }
            */

            LoadCharactersData();
            
            await AwaitableUtils.CompletedAwaitable;
        }

        protected virtual async Awaitable Execute()
        {
            await AwaitableUtils.AwaitableUntil(() => IsDone, CancellationToken.None);
        }
        
        protected virtual async Awaitable OnEnd()
        {
            await AwaitableUtils.CompletedAwaitable;
        }

        public void SelectCharacter(CharacterData characterData, bool characterChanged)
        {
            if (characterData == null)
                return;
            
            //Debug.LogError($"SetPlayer property {ConstID.Global.PlayerPropertyCharacterData} for character {characterData.AgentName}");
            Awaitable aw = CurrentPlayer.UpdatePlayerProperty(ConstID.Global.PlayerPropertyCharacterData, new PlayerProperty(characterData.ID));
            aw.Run();
            
            SelectedCharacter = characterData;
            OnCharacterSelected?.Invoke(CurrentPlayer, characterData, characterChanged);
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