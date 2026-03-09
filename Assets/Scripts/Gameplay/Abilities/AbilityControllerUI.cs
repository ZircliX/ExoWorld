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
        private AbilityIconReference secondaryIcon;

        private void Awake()
        {
            primaryTarget.ClearChildren();
            secondaryTarget.ClearChildren();
        }

        private void OnEnable()
        {
            abilityController.OnAbilitiesChanged += OnAbilitiesChanged;
        }

        private void OnDisable()
        {
            abilityController.OnAbilitiesChanged -= OnAbilitiesChanged;
        }

        private void OnAbilitiesChanged(IAbility primary, IAbility secondary)
        {
            if (primaryIcon != null)
                Destroy(primaryIcon.gameObject);
            if (secondaryIcon != null)
                Destroy(secondaryIcon.gameObject);
            
            primaryIcon = Instantiate(primary.Data.Icon, primaryTarget);
            secondaryIcon = Instantiate(secondary.Data.Icon, secondaryTarget);
        }
    }
}