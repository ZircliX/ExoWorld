using OverBang.GameName.Gameplay.Interface;
using TMPro;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PlayerInteractionUI : MonoBehaviour
    {
        [SerializeField] private PlayerInteraction playerInteraction;
        [SerializeField] private TMP_Text interactionText;

        private void OnEnable()
        {
            playerInteraction.OnNewInteractable += UpdateInteractableUI;
        }

        private void OnDisable()
        {
            playerInteraction.OnNewInteractable -= UpdateInteractableUI;
        }

        private void UpdateInteractableUI(IInteractable interactable)
        {
            if (interactable == null)
            {
                interactionText.text = string.Empty;
                return;
            }
            
            interactionText.text = interactable.InteractionText;
        }
    }
}