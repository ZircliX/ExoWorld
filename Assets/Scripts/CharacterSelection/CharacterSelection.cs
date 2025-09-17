using System.Collections.Generic;
using DG.Tweening;
using OverBang.GameName.Core.Characters;
using UnityEngine;

namespace OverBang.GameName.Offline.CharacterSelectionSystem
{
    public class CharacterSelection : MonoBehaviour
    {
        [SerializeField] private List<CharacterData> agentData;
        [SerializeField] private CharacterCard characterCardPrefab;
        
        [SerializeField] private Transform agentCardContainer;
        [SerializeField] private CanvasGroup canvasGroup;
        
        private List<CharacterCard> agentCards;
        private CharacterSelectionManager.SelectionSettings currentSettings;
        
        private void Awake()
        {
            agentCards = new List<CharacterCard>(4);
            canvasGroup.alpha = 0;
        }
        
        private void SpawnCards(List<CharacterData> validAgents)
        {
            foreach (CharacterData agent in validAgents)
            {
                CharacterCard card = Instantiate(characterCardPrefab, agentCardContainer);
                agentCards.Add(card);
                UpdateAgentCard(card, agent);
            }
        }

        private void UpdateAgentCard(CharacterCard card, CharacterData data)
        {
            card.Setup(data);
        }
        
        public void StartSelection(CharacterSelectionManager.SelectionSettings settings)
        {
            canvasGroup?.DOFade(1, 0.25f);
            currentSettings = settings;

            List<CharacterData> validAgents = SortCharacters(settings);
            
            switch (settings.Type)
            {
                case CharacterSelectionManager.SelectionSettings.SelectionType.Random:
                {
                    int randomIndex = Random.Range(0, validAgents.Count);
                    CharacterData randomAgent = agentData[randomIndex];
                    CharacterSelectionManager.Instance.PlayerSelected(randomAgent);
                    Destroy(gameObject, 0.5f);
                    break;
                }
                default:
                    SpawnCards(validAgents);
                    break;
            }
        }

        private List<CharacterData> SortCharacters(CharacterSelectionManager.SelectionSettings settings)
        {
            List<CharacterData> validAgents = new List<CharacterData>(agentData);
            if (settings.ClassLimitation != CharacterClasses.None)
            {
                validAgents = agentData.FindAll(data => data.CharacterClass == currentSettings.ClassLimitation);
            }
            
            return validAgents;
        }
    }
}