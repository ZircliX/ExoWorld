using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay
{
    public class HelpPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private GameObject tab;

        private bool state;

        public void Open(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if (state) return;
            
            FadePanel(canvasGroup, true);
            tab.SetActive(false);
        }
        
        public void Close(InputAction.CallbackContext ctx)
        {
            if (!ctx.performed) return;
            if (!state) return;
            
            FadePanel(canvasGroup, false);
            tab.SetActive(true);
        }
        
        private void FadePanel(CanvasGroup group, bool visible)
        {
            group.DOFade(visible ? 1 : 0, 0.3f).OnComplete(() =>
            {
                state = visible;
            });
        }
    }
}