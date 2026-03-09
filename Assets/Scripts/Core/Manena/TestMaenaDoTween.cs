using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Manena
{
    public class TestMaenaDoTween : MonoBehaviour
    {
        [SerializeField] private Ease easingCurve;
        [SerializeField] private Image image;

        private void Start()
        {
            transform.DOScaleX(0, 1.5f).SetEase(easingCurve);
        }
    }
}