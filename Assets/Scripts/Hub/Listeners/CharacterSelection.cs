using System.Collections.Generic;
using DG.Tweening;
using OverBang.GameName.Core.Characters;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public class CharacterSelection : HubListener
    {
        [SerializeField] private CharacterCardUI characterCardUIPrefab;
        
        [SerializeField] private Transform agentCardContainer;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private List<CharacterCardUI> agentCards;
        private HubPhase.SelectionSettings currentSettings;
        private void Awake()
        {
            agentCards = new List<CharacterCardUI>(4);
        }
        
        protected internal override void OnInit(HubPhase phase)
        {
            ChangeEnabledState(true);
            phase.OnAvailableCharacterAdded += AddCharacter;
        }

        protected internal override void OnRelease(HubPhase phase)
        {
            ChangeEnabledState(false);
            phase.OnAvailableCharacterAdded -= AddCharacter;
        }
        
        private void AddCharacter(CharacterData characterData)
        {
            if (Mathf.Approximately(canvasGroup.alpha, 0)) ChangeEnabledState(true);
            
            //Debug.Log($" Adding character {characterData.AgentName} to selection UI");
            CharacterCardUI cardUI = Instantiate(characterCardUIPrefab, agentCardContainer);
            cardUI.Setup(characterData, this);
            
            agentCards.Add(cardUI);
        }
        
        public void SelectCharacter(CharacterData characterData)
        {
            //Debug.Log(" [Character Selection] SelectCharacter + " + characterData.AgentName);
            ChangeEnabledState(false);
            current?.SelectCharacter(characterData);
        }
        
        private void ChangeEnabledState(bool enabled)
        {
            canvasGroup.DOFade(enabled ? 1f : 0f, 0.5f);
            canvasGroup.interactable = enabled;
            canvasGroup.blocksRaycasts = enabled;
        }
    }
}