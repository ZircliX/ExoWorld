using DG.Tweening;
using KBCore.Refs;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
{
    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasPanelGroup : MonoBehaviour
    {
        [SerializeField, Self] private CanvasGroup canvasGroup;
        [SerializeField] private float animationDuration = 0.3f;
        [SerializeField] private bool closeOnAwake = true;
        [SerializeField] private bool interactable = true;

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            if (closeOnAwake) Close(false);
        }

        public void Open(bool animate = true)
        {
            canvasGroup.DOKill(true);

            if (animate)
            {
                canvasGroup.DOFade(1, animationDuration).OnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = interactable;
                    canvasGroup.interactable = interactable;
                });
            }
            else
            {
                canvasGroup.alpha = 1;
                canvasGroup.blocksRaycasts = interactable;
                canvasGroup.interactable = interactable;
            }
        }

        public void Close(bool animate = true)
        {
            canvasGroup.DOKill(true);

            if (animate)
            {
                canvasGroup.DOFade(0, animationDuration).OnComplete(() =>
                {
                    canvasGroup.blocksRaycasts = false;
                    canvasGroup.interactable = false;
                });
            }
            else
            {
                canvasGroup.alpha = 0;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.interactable = false;
            }
        }
    }
}