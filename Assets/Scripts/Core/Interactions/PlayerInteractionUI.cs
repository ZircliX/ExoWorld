using DG.Tweening;
using TMPro;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Interactions
{
    public class PlayerInteractionUI : MonoBehaviour
    {
        [SerializeField] private PlayerInteraction playerInteraction;
        [SerializeField] private TMP_Text interactionText;
        [SerializeField] private GameObject inputKey;
        [SerializeField] private CanvasGroup canvasGroup;

        private void OnEnable()
        {
            playerInteraction.OnNewInteractable += UpdateInteractableUI;
            playerInteraction.OnItemDropped += UpdateInteractableUI;
            playerInteraction.OnItemPickup += UpdateInteractableUI;
            canvasGroup.alpha = 0;
        }

        private void OnDisable()
        {
            playerInteraction.OnNewInteractable -= UpdateInteractableUI;
            playerInteraction.OnItemDropped -= UpdateInteractableUI;
            playerInteraction.OnItemPickup -= UpdateInteractableUI;
        }

        private void UpdateInteractableUI(InteractableData interactable)
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

            inputKey.SetActive(interactable.Instance.CanInteract);
            interactionText.text = interactable.Instance.InteractionText == string.Empty ? "Interagir" : interactable.Instance.InteractionText;
        }

        private void LateUpdate()
        {
            InteractableData interactable = playerInteraction.CurrentInteractable;
            if (interactable != null)
            {
                canvasGroup.transform.position = interactable.Instance.UIPosition;
                canvasGroup.transform.forward = playerInteraction.InteractionCamera.transform.forward;
            }
        }
    }
}