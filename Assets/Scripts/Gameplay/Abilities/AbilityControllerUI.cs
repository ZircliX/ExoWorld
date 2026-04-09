using DG.Tweening;
using Helteix.Tools;
using OverBang.ExoWorld.Core.Abilities;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class AbilityControllerUI : MonoBehaviour
    {
        [SerializeField] private AbilityController abilityController;
        [SerializeField] private Transform primaryTarget;
        [SerializeField] private Transform secondaryTarget;
        
        private AbilityIconReference primaryIcon;
        private IAbility primaryAbility;
        private AbilityIconReference secondaryIcon;
        private IAbility secondaryAbility;

        private void Awake()
        {
            primaryTarget.ClearChildren();
            secondaryTarget.ClearChildren();
        }

        private void OnEnable()
        {
            abilityController.OnAbilitiesChanged += OnAbilitiesChanged;
            abilityController.OnAbilityStarted += OnAbilityStarted;
            abilityController.OnAbilityEnded += OnAbilityEnded;
            abilityController.OnAbilityCooldownEnded += OnAbilityCooldownEnded;
        }

        private void OnDisable()
        {
            abilityController.OnAbilitiesChanged -= OnAbilitiesChanged;
            abilityController.OnAbilityStarted -= OnAbilityStarted;
            abilityController.OnAbilityEnded -= OnAbilityEnded;
            abilityController.OnAbilityCooldownEnded -= OnAbilityCooldownEnded;
        }

        private void OnAbilitiesChanged(IAbility primary, IAbility secondary)
        {
            if (primaryIcon != null)
                Destroy(primaryIcon.gameObject);
            if (secondaryIcon != null)
                Destroy(secondaryIcon.gameObject);
            
            primaryIcon = Instantiate(primary.Data.Icon, primaryTarget);
            secondaryIcon = Instantiate(secondary.Data.Icon, secondaryTarget);

            primaryAbility = primary;
            secondaryAbility = secondary;
        }

        private void OnAbilityStarted(IAbility ability)
        {
            AbilityIconReference icon = ability == primaryAbility ? primaryIcon : secondaryIcon;
            
            Debug.Log($"Ability {ability.Data.Name} started, duration: {ability.Duration}");
            icon.Begin(ability.Duration);
        }
        
        private void OnAbilityEnded(IAbility ability)
        {
            AbilityIconReference icon = ability == primaryAbility ? primaryIcon : secondaryIcon;
            
            Debug.Log($"Ability {ability.Data.Name} ended, cooldown: {ability.Data.Cooldown}");
            icon.End(ability.Data.Cooldown);
        }
        
        private void OnAbilityCooldownEnded(IAbility ability)
        {
            AbilityIconReference icon = ability == primaryAbility ? primaryIcon : secondaryIcon;

            icon.CooldownEnd();
        }
    }
}