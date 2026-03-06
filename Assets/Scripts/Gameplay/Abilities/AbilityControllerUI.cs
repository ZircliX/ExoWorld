using OverBang.ExoWorld.Core.Abilities;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class AbilityControllerUI : MonoBehaviour
    {
        [SerializeField] private AbilityController abilityController;
        [SerializeField] private Transform primaryTarget;
        [SerializeField] private Transform secondaryTarget;
        
        private AbilityIconReference primaryIcon;
        private AbilityIconReference secondaryIcon;

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
                Destroy(primaryIcon);
            if (secondaryIcon != null)
                Destroy(secondaryIcon);
            
            primaryIcon = Instantiate(primary.Data.Icon, primaryTarget);
            secondaryIcon = Instantiate(secondary.Data.Icon, secondaryTarget);
        }
    }
}