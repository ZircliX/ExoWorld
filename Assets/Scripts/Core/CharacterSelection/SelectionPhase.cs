using System;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Phases;
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
            
            public PlayerProfile playerProfile;
        }
        
        protected readonly SelectionSettings settings;

        public PlayerProfile PlayerProfile { get; private set; }
        
        public event Action<PlayerProfile> OnCharacterSelected;
        public event Action<CharacterData> OnAvailableCharacterAdded;

        public SelectionPhase(SelectionSettings selectionSettings)
        {
            settings = selectionSettings;
        }

        public virtual Awaitable OnBegin()
        {
            PlayerProfile.sessionPlayer.SetProperty("Character", new PlayerProperty(string.Empty));
            return null;
        }

        public virtual Awaitable OnEnd(bool success)
        {
            if (success)
            {
                PlayerProfile.sessionPlayer.SetProperty("Character", new PlayerProperty(PlayerProfile.characterData.ID));
            }

            return null;
        }

        public void SelectCharacter(CharacterData characterData)
        {
            PlayerProfile = new PlayerProfile()
            {
                characterData = characterData
            };
            
            OnCharacterSelected?.Invoke(PlayerProfile);
        }
        
        public void StartCharacterSelection()
        {
            CharacterData[] characters = Resources.LoadAll<CharacterData>("Characters");

            for (int i = 0; i < characters.Length; i++)
            {
                //Debug.Log("HubPhase: Processing loaded character " + ctx.name);
                if(!characters[i].CharacterClass.Matches(settings.availableClasses))
                    return;
                OnAvailableCharacterAdded?.Invoke(characters[i]);
            }
        }
    }
}