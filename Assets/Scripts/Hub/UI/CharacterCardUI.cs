using KBCore.Refs;
using OverBang.GameName.Core.Characters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.GameName.Hub
{
    public class CharacterCardUI : MonoBehaviour
    {
        [field: SerializeField] public Image CardSprite { get; private set; }
        [field: SerializeField] public TMP_Text CardName { get; private set; }
        [field: SerializeField] public TMP_Text CardAvailability { get; private set; }
        [field: SerializeField, Child] public Button SelectButton { get; private set; }
        [field: SerializeField, Self] public CanvasGroup CardCanvasGroup { get; private set; }

        private CharacterData characterData;
        private CharacterSelection _characterSelection;

        private void OnValidate() => this.ValidateRefs();

        private void OnEnable() => SelectButton.onClick.AddListener(OnButtonClick);

        private void OnDisable() => SelectButton.onClick.RemoveListener(OnButtonClick);

        private void OnButtonClick()
        {
            _characterSelection.SelectCharacter(characterData);
        }

        public void SetSprite(Sprite sprite)
        {
            CardSprite.sprite = sprite;
        }

        public void SetName(string newName)
        {
            CardName.text = newName;
        }
        
        public void SetAvailability(bool availability)
        {
            CardAvailability.text = availability ? "Available" : "Unavailable";
            
            CardCanvasGroup.alpha = availability ? 1f : 0.5f;
            CardCanvasGroup.interactable = availability;
            CardCanvasGroup.blocksRaycasts = availability;
        }

        public void Setup(CharacterData character, CharacterSelection selection)
        {
            characterData = character;
            _characterSelection = selection;
            
            SetSprite(character.AgentSprite);
            SetName(character.AgentName);
            SetAvailability(true);
        }
    }
}