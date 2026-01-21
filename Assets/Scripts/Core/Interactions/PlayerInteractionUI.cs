using DG.Tweening;
using TMPro;
using UnityEngine;

namespace OverBang.ExoWorld.Core
{
    public class PlayerInteractionUI : MonoBehaviour
    {
        [SerializeField] private PlayerInteraction playerInteraction;
        [SerializeField] private TMP_Text interactionText;
        [SerializeField] private CanvasGroup canvasGroup;

        private void OnEnable()
        {
            playerInteraction.OnNewInteractable += UpdateInteractableUI;
            canvasGroup.alpha = 0;
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
                canvasGroup.DOKill();
                canvasGroup.DOFade(0, .2f);
                return;
            }

            canvasGroup.DOKill();
            canvasGroup.DOFade(1, .2f);
            
            interactionText.text = interactable.InteractionText == string.Empty ? "Intéragir" : interactable.InteractionText;
        }

        private void LateUpdate()
        {
            IInteractable interactable = playerInteraction.CurrentInteractable;
            if (interactable != null)
            {
                canvasGroup.transform.position = interactable.UIPosition;
                canvasGroup.transform.forward = playerInteraction.InteractionCamera.transform.forward;
            }
        }
    }
}