using DG.Tweening;
using OverBang.ExoWorld.Core.Metrics;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
{
    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance;

        [SerializeField] private CanvasGroup canvasGroup;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
                Destroy(this);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Initialize()
        {
            if (Instance == null)
            {
                Instance = Instantiate(GameMetrics.Global.LoadingScreen);
                DontDestroyOnLoad(Instance.gameObject);
            }
        }
        
        public async Awaitable Show()
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            canvasGroup.DOFade(1, 0.5f).SetUpdate(true);
            await Awaitable.WaitForSecondsAsync(0.5f);
        }

        public async Awaitable Hide()
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
            canvasGroup.DOFade(0, 0.5f).SetUpdate(true);
            await Awaitable.WaitForSecondsAsync(0.5f);
        }
    }
}