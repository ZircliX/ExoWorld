using DG.Tweening;
using Helteix.ChanneledProperties.Priorities;
using OverBang.ExoWorld.Core;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Menus;
using OverBang.ExoWorld.Gameplay.HUB.Listeners;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.HUB
{
    public class PlayerSelectionUI : BasePanel
    {
        [SerializeField, Required] private PlayerSelection playerSelection;
        
        [Header("Navigation Buttons")]
        [SerializeField, Space] private Button selectButton;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        
        [Header("Character Infos")]
        [SerializeField, Space] private TMP_Text characterNameText;
        [SerializeField] private TMP_Text characterCategoryText;
        [SerializeField] private Image characterImage;
        [SerializeField] private TMP_Text characterDescriptionText;
        
        [Header("Ability Infos")]
        [SerializeField, Space] private Button primaryAbilityButton;
        [SerializeField] private Button secondaryAbilityButton;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color unselectedColor;
        
        [SerializeField] private TMP_Text abilityNameText;
        [SerializeField] private TMP_Text abilityDescriptionText;
        
        private CharacterData currentCharacterData;
        private int currentCharacterIndex = 0;
        private bool abilitySelected = true; // true = primary, false = secondary

        protected override void Awake()
        {
            base.Awake();
            GameController.CursorLockModePriority.AddPriority(this, PriorityTags.High, CursorLockMode.Locked);
            GameController.CursorVisibleStatePriority.AddPriority(this, PriorityTags.High, false);
            
            playerSelection.OnCharactersLoaded += OnLoaded;
            
            selectButton.onClick.AddListener(ConfirmSelection);
            previousButton.onClick.AddListener(() => SwitchCharacter(-1));
            nextButton.onClick.AddListener(() => SwitchCharacter(1));
            
            primaryAbilityButton.onClick.AddListener( () => UpdateAbility(true));
            secondaryAbilityButton.onClick.AddListener( () => UpdateAbility(false));
        }

        private void OnLoaded()
        {
            playerSelection.OnCharactersLoaded -= OnLoaded;
            currentCharacterData = playerSelection.characterDatas[currentCharacterIndex];
            ChangeEnabledState(true);
            UpdateCharacterUI();
        }

        private void ChangeEnabledState(bool enabled)
        {
            GameController.CursorLockModePriority.Write(this, enabled ? CursorLockMode.None : CursorLockMode.Confined);
            GameController.CursorVisibleStatePriority.Write(this, enabled);
            
            canvasGroup.DOFade(enabled ? 1f : 0f, 0.5f);
            canvasGroup.interactable = enabled;
            canvasGroup.blocksRaycasts = enabled;
        }

        private void SwitchCharacter(int direction)
        {
            int characterCount = playerSelection.characterDatas.Count;
            currentCharacterIndex = (currentCharacterIndex + direction) % characterCount;
    
            if (currentCharacterIndex < 0)
                currentCharacterIndex += characterCount;
            
            currentCharacterData = playerSelection.characterDatas[currentCharacterIndex];
            UpdateCharacterUI();
        }

        private void UpdateCharacterUI()
        {
            characterNameText.text = currentCharacterData.Name;
            characterCategoryText.text = currentCharacterData.CharacterClass.ToString();
            characterImage.sprite = currentCharacterData.Sprite;
            characterDescriptionText.text = currentCharacterData.Description;
            
            primaryAbilityButton.image.sprite = currentCharacterData.PrimaryAbility.Icon;
            secondaryAbilityButton.image.sprite = currentCharacterData.SecondaryAbility.Icon;
            
            UpdateAbility(abilitySelected);
        }

        private void UpdateAbility(bool primary)
        {
            abilitySelected = primary;
            AbilityData abilityData = primary ? currentCharacterData.PrimaryAbility : currentCharacterData.SecondaryAbility;
            
            primaryAbilityButton.image.color = primary ? selectedColor : unselectedColor;
            secondaryAbilityButton.image.color = primary ? unselectedColor : selectedColor;
            
            abilityNameText.text = abilityData.Name;
            abilityDescriptionText.text = abilityData.Description;
        }

        private void ConfirmSelection()
        {
            ChangeEnabledState(false);
            GameController.CursorLockModePriority.RemovePriority(this);
            GameController.CursorVisibleStatePriority.RemovePriority(this);
            playerSelection.ConfirmSelection(currentCharacterData);
        }
    }
}