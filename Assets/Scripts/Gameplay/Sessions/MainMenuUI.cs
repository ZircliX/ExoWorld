using DG.Tweening;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class MainMenuUI : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mainMenuCanvas;
        [SerializeField] private CanvasGroup joinCanvas;
        [SerializeField] private CanvasGroup createCanvas;
        
        private CanvasGroup currentCanvas;

        private void Awake()
        {
            currentCanvas = mainMenuCanvas;
        }

        public void OpenMenu()
        {
            OpenCanvas(mainMenuCanvas, true);
            currentCanvas = mainMenuCanvas;
        }
        
        public void OpenJoin()
        {
            OpenCanvas(joinCanvas, true);
            currentCanvas = joinCanvas;
        }
        
        public void OpenCreate()
        {
            OpenCanvas(createCanvas, true);
            currentCanvas = createCanvas;
        }
        
        private void OpenCanvas(CanvasGroup canvas, bool state)
        {
            canvas.DOFade(state ? 1 : 0, 0.3f).OnComplete(() =>
            {
                canvas.blocksRaycasts = state;
                canvas.interactable = state;
            });
            
            currentCanvas.DOFade(state ? 0 : 1, 0.3f).OnComplete(() =>
            {
                canvas.blocksRaycasts = !state;
                canvas.interactable = !state;
            });
        }
    }
}