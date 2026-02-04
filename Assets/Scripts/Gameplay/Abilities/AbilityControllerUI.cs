using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class AbilityControllerUI : MonoBehaviour
    {
        [SerializeField] private AbilityController abilityController;
        [SerializeField] private Image primaryImage;
        [SerializeField] private Image secondaryImage;

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
            primaryImage.sprite = primary.Data.Icon;
            secondaryImage.sprite = secondary.Data.Icon;
        }
    }
}