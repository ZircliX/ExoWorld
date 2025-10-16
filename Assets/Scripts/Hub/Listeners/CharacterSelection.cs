using System.Collections.Generic;
using DG.Tweening;
using Helteix.ChanneledProperties.Priorities;
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
            GameController.CursorLockModePriority.AddPriority(this, PriorityTags.Highest, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.AddPriority(this, PriorityTags.Highest, false);
            phase.OnAvailableCharacterAdded += AddCharacter;
        }

        protected internal override void OnRelease(HubPhase phase)
        {
            GameController.CursorLockModePriority.RemovePriority(this);
            GameController.CursorVisibleStatePriority.RemovePriority(this);
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
            current?.SelectLocalCharacter(characterData);
        }
        
        private void ChangeEnabledState(bool enabled)
        {
            GameController.CursorLockModePriority.Write(this, enabled ? CursorLockMode.None : CursorLockMode.Confined);
            GameController.CursorVisibleStatePriority.Write(this, enabled);
            
            canvasGroup.DOFade(enabled ? 1f : 0f, 0.5f);
            canvasGroup.interactable = enabled;
            canvasGroup.blocksRaycasts = enabled;
        }
    }
}