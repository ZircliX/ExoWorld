using DG.Tweening;
using Helteix.Singletons.SceneServices;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Menus
{
    public class LoadingUI : SceneService<LoadingUI>
    {
        [SerializeField] private Image loadingImage;
        [SerializeField, Self] private CanvasPanelGroup canvasPanelGroup;

        private bool isOpen;
        private float time;

        private void OnValidate() => this.ValidateRefs();

        private void Awake()
        {
            loadingImage.transform.DORotate(new Vector3(0, 0, -360), 1.5f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1);
        }

        private void Update()
        {
            if (isOpen)
                time += Time.deltaTime;

            if (time > 10)
            {
                Close();
            }
                
        }

        public void Open()
        {
            canvasPanelGroup.Open();
            isOpen = true;
            time = 0;
        }
        
        public void Close()
        {
            canvasPanelGroup.Close();
            isOpen = false;
            time = 0;
        }
    }
}