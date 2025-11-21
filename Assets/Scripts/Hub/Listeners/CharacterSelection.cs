using System.Collections.Generic;
using DG.Tweening;
using Helteix.ChanneledProperties.Priorities;
using Helteix.Tools;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public class CharacterSelection : MonoPhaseListener<HubPhase>
    {
        [SerializeField] private CharacterCardUI characterCardUIPrefab;
        
        [SerializeField] private Transform agentCardContainer;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private List<CharacterCardUI> agentCards;
        private SelectionPhase.SelectionSettings currentSettings;
        private void Awake()
        {
            agentCards = new List<CharacterCardUI>(4);
        }
        
        protected override void OnBegin(HubPhase phase)
        {
            GameController.CursorLockModePriority.AddPriority(this, PriorityTags.Highest, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.AddPriority(this, PriorityTags.Highest, false);
            agentCardContainer.ClearChildren();
            
            foreach (CharacterData characterData in phase.AvailableCharacters)
            {
                //Debug.Log($"Adding character {characterData.AgentName} to selection UI");
                CharacterCardUI cardUI = Instantiate(characterCardUIPrefab, agentCardContainer);
                cardUI.Setup(characterData, this);
            
                agentCards.Add(cardUI);
            }
            
            if (phase.Settings.selectionType == SelectionPhase.SelectionType.Pick)
            {
                EnableUI();
            }
        }

        protected override void OnEnd(HubPhase phase)
        {
            GameController.CursorLockModePriority.RemovePriority(this);
            GameController.CursorVisibleStatePriority.RemovePriority(this);
            ChangeEnabledState(false);
        }
        
        private void EnableUI()
        {
            ChangeEnabledState(true);
        }
        
        public void SelectCharacter(CharacterData characterData)
        {
            //Debug.Log(" [Character Selection] SelectCharacter + " + characterData.AgentName);
            ChangeEnabledState(false);
            CurrentPhase.SelectCharacter(characterData);
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