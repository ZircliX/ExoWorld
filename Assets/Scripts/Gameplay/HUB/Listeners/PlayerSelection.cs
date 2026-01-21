using System;
using System.Collections.Generic;
using OverBang.GameName.Core;
using OverBang.GameName.Gameplay;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public class PlayerSelection : MonoPhaseListener<HubPhase>
    {
        [SerializeField] private LoadoutTable loadoutTable;
        public List<CharacterData> characterDatas { get; private set; }
        public event Action OnCharactersLoaded;

        private void Awake()
        {
            characterDatas = new List<CharacterData>(4);
        }

        protected override void OnBegin(HubPhase phase)
        {
            if (phase.Settings.selectionType != SelectionPhase.SelectionType.Pick)
                return;
            
            foreach (CharacterData characterData in phase.AvailableCharacters)
                characterDatas.Add(characterData);
            
            OnCharactersLoaded?.Invoke();
            Debug.Log("Loading Complete");
        }
        
        public void ConfirmSelection(CharacterData characterData)
        {
            //Debug.Log(" [Character Selection] SelectCharacter + " + characterData.AgentName);
            CurrentPhase.SelectCharacter(characterData, true);
            loadoutTable.StartLoadoutSelection();
        }
    }
}