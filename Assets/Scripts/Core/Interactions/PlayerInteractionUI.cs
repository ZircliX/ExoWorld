using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Interactions
{
    public class PlayerInteractionUI : MonoBehaviour
    {
        [SerializeField] private PlayerInteraction playerInteraction;
        [SerializeField] private TMP_Text interactionText;
        [SerializeField] private Transform inputKey;
        [SerializeField] private CanvasGroup canvasGroup;

        private void OnEnable()
        {
            playerInteraction.OnNewInteractable += UpdateInteractableUI;
            playerInteraction.OnItemDropped += UpdateInteractableUI;
            playerInteraction.OnItemPickup += UpdateInteractableUI;
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
                //interactionText.text = string.Empty;
                canvasGroup.DOKill();
                canvasGroup.DOFade(0, .2f);
                RebuildLayout();
                return;
            }

            canvasGroup.DOKill();
            canvasGroup.DOFade(1, .2f);

            inputKey.gameObject.SetActive(interactable.Instance.CanInteract);
            interactionText.text = interactable.Instance.InteractionText == string.Empty ? "Interagir" : interactable.Instance.InteractionText;
            RebuildLayout();
        }

        private void RebuildLayout()
        {
            if (canvasGroup != null)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(canvasGroup.GetComponent<RectTransform>());
            }
        }
    }
}