using System;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Metrics;
using OverBang.GameName.Core.Phases;
using OverBang.GameName.Managers;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Core.CharacterSelection
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
        
        protected readonly SelectionSettings settings;


        public event Action<IPlayer, CharacterData> OnCharacterSelected;
        public event Action<CharacterData> OnAvailableCharacterAdded;

        public SelectionPhase(SelectionSettings selectionSettings)
        {
            settings = selectionSettings;
        }

        public virtual async Awaitable OnBegin()
        {
            IPlayer globalCurrentPlayer = SessionManager.Global.CurrentPlayer;
            if (!globalCurrentPlayer.Properties.ContainsKey(ConstID.Global.PlayerPropertyCharacterData))
            {
                globalCurrentPlayer.SetProperty(ConstID.Global.PlayerPropertyCharacterData,
                    new PlayerProperty(string.Empty));
            }
        }

        public virtual async Awaitable OnEnd(bool success)
        {
        }

        public  void SelectCharacter(CharacterData characterData)
        {
            IPlayer globalCurrentPlayer = SessionManager.Global.CurrentPlayer;
            globalCurrentPlayer.SetProperty(ConstID.Global.PlayerPropertyCharacterData, new PlayerProperty(characterData.ID));
            OnCharacterSelected?.Invoke(globalCurrentPlayer, characterData);
        }
        
        public void StartCharacterSelection()
        {
            CharacterData[] characters = Resources.LoadAll<CharacterData>("Characters");

            for (int i = 0; i < characters.Length; i++)
            {
                if(!characters[i].CharacterClass.Matches(settings.availableClasses))
                    continue;
                OnAvailableCharacterAdded?.Invoke(characters[i]);
            }
        }
    }
}