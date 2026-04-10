using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Gameplay.Loadout;
using OverBang.ExoWorld.Gameplay.Phase;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.HUB.Listeners
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
            {
                characterDatas.Add(characterData);
                Debug.Log($"Loaded character data: {characterData.Name}");
            }
            
            OnCharactersLoaded?.Invoke();
            //Debug.Log("Loading Complete");
        }
        
        public void ConfirmSelection(CharacterData characterData)
        {
            //Debug.Log(" [Character Selection] SelectCharacter + " + characterData.AgentName);
            CurrentPhase.SelectCharacter(characterData, true);
            loadoutTable.StartLoadoutSelection();
        }
    }
}