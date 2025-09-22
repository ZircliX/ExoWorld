using System;
using LTX.Singletons;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.CharacterSelection
{
    public class CharacterSelectionManager : MonoSingleton<CharacterSelectionManager>
    {
        [System.Serializable]
        public struct SelectionSettings
        {
            public enum SelectionType
            {
                Random,
                Pick
            }
            
            public SelectionType Type;
            public CharacterClasses ClassLimitation;
        }
        
        public event Action<CharacterData> OnCharacterSelected;
        private GameObject currentSelection;

        public void StartCharacterSelection(SelectionSettings settings, Action<CharacterData> predicate)
        {
            StopCharacterSelection(predicate);
            
            OnCharacterSelected += predicate;
            currentSelection = Instantiate(GameController.Metrics.CharacterSelectionPrefab);
            currentSelection.GetComponent<CharacterSelection>().StartSelection(settings);
        }


        public void StopCharacterSelection(Action<CharacterData> predicate)
        {
            OnCharacterSelected -= predicate;
            
            if (currentSelection == null) return;
            Destroy(currentSelection);
        }

        public void PlayerSelected(CharacterData character)
        {
            OnCharacterSelected?.Invoke(character);
        }
    }
}