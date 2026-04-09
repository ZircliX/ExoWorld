using Helteix.ChanneledProperties.Priorities;
using Helteix.Tools;
using OverBang.ExoWorld.Core;
using OverBang.ExoWorld.Core.Abilities;
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
        [Space]
        [SerializeField] private Button primaryAbilityButton;
        [SerializeField] private Button secondaryAbilityButton;
        [SerializeField] private Transform primaryAbilityTarget;
        [SerializeField] private Transform secondaryAbilityTarget;
        [SerializeField] private Color selectedColor;
        [SerializeField] private Color unselectedColor;
        
        [SerializeField] private TMP_Text abilityNameText;
        [SerializeField] private TMP_Text abilityDescriptionText;

        private AbilityIconReference primaryIcon;
        private AbilityIconReference secondaryIcon;
        
        private CharacterData currentCharacterData;
        private int currentCharacterIndex;
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
            
            primaryAbilityButton.transform.ClearChildren();
            secondaryAbilityButton.transform.ClearChildren();
        }

        private void OnLoaded()
        {
            playerSelection.OnCharactersLoaded -= OnLoaded;
            currentCharacterData = playerSelection.characterDatas[currentCharacterIndex];
            ChangeEnabledState(true);
            UpdateCharacterUI();
        }

        private void ChangeEnabledState(bool newState)
        {
            //if (selectButton.interactable == newState) return;
            
            GameController.CursorLockModePriority.Write(this, newState ? CursorLockMode.None : CursorLockMode.Confined);
            GameController.CursorVisibleStatePriority.Write(this, newState);

            if (newState)
            {
                canvasGroup.Open();
            }
            else
            {
                canvasGroup.Close();
            }
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
            if (characterImage != null)
                characterImage.sprite = currentCharacterData.Sprite;
            characterDescriptionText.text = currentCharacterData.Description;
            
            if (primaryIcon != null)
                Destroy(primaryIcon.gameObject);
            if (secondaryIcon != null)
                Destroy(secondaryIcon.gameObject);
            
            primaryIcon = Instantiate(currentCharacterData.PrimaryAbility.Icon, primaryAbilityTarget);
            secondaryIcon = Instantiate(currentCharacterData.SecondaryAbility.Icon, secondaryAbilityTarget);

            primaryAbilityButton.targetGraphic = primaryIcon.AbilityIcon;
            secondaryAbilityButton.targetGraphic = secondaryIcon.AbilityIcon;
            
            UpdateAbility(abilitySelected);
        }

        private void UpdateAbility(bool primary)
        {
            abilitySelected = primary;
            AbilityData abilityData = primary ? currentCharacterData.PrimaryAbility : currentCharacterData.SecondaryAbility;

            return;
            primaryAbilityButton.image.color = primary ? selectedColor : unselectedColor;
            secondaryAbilityButton.image.color = primary ? unselectedColor : selectedColor;
            
            abilityNameText.text = abilityData.Name;
            abilityDescriptionText.text = abilityData.Description;
        }

        private void ConfirmSelection()
        {
            selectButton.interactable = false;
            
            ChangeEnabledState(false);
            GameController.CursorLockModePriority.RemovePriority(this);
            GameController.CursorVisibleStatePriority.RemovePriority(this);
            playerSelection.ConfirmSelection(currentCharacterData);
        }
    }
}