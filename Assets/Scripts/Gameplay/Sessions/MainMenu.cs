using System;
using DG.Tweening;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private CanvasGroup mainMenu;
        [SerializeField] private CanvasGroup join;
        [SerializeField] private CanvasGroup create;
        
        private CanvasGroup currentCanvas;

        private void Awake()
        {
            currentCanvas = mainMenu;
        }

        public void OpenMenu()
        {
            OpenCanvas(mainMenu, true);
            currentCanvas = mainMenu;
        }
        
        public void OpenJoin()
        {
            OpenCanvas(join, true);
            currentCanvas = join;
        }
        
        public void OpenCreate()
        {
            OpenCanvas(create, true);
            currentCanvas = create;
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