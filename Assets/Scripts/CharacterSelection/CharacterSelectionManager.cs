using System;
using LTX.Singletons;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Offline.CharacterSelectionSystem
{
    public class CharacterSelectionManager : MonoSingleton<CharacterSelectionManager>
    {
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

        public void StartCharacterSelection(SelectionSettings settings, Action<CharacterData> predicate)
        {
            Debug.Log("Starting character selection");
            
            OnCharacterSelected += predicate;
            GameObject selection = Instantiate(GameController.Metrics.CharacterSelectionPrefab);
            selection.GetComponent<CharacterSelection>().StartSelection(settings);
        }

        public void PlayerSelected(CharacterData character)
        {
            OnCharacterSelected?.Invoke(character);
        }
    }
}