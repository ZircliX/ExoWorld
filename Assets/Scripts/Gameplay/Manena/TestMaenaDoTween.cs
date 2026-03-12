using DG.Tweening;
using TMPro;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Manena
{
    public class TestMaenaDoTween : MonoBehaviour
    {
        [SerializeField] private CanvasGroup questCompleteCanvasGroup;
        [SerializeField] private CanvasGroup questRewardCanvasGroup;
        [SerializeField] private TMP_Text questCompleteTitleText;
        [SerializeField] private TMP_Text questCompleteNameText;
        [SerializeField] private TMP_Text questCompleteRewardText;
        
        [SerializeField, Range(0,1)] private float openingXScaleDuration;
        [SerializeField, Range(0,1)] private float openingYScaleDuration;
        [SerializeField] private float openingDuration;
        [SerializeField] private Ease openingEasingCurve;

        [SerializeField, Range(0,1)] private float endingXScaleDuration;
        [SerializeField, Range(0,1)] private float endingYScaleDuration;
        [SerializeField] private float endingDuration;
        [SerializeField] private Ease endingEasingCurve;

        public void DoAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            questCompleteCanvasGroup.transform.localScale = new Vector3(0.01f, 0, 1);
            
            //Show Quest Completed BG
            sequence.Append(questCompleteCanvasGroup.DOFade(1, openingDuration));
            sequence.Join(questCompleteCanvasGroup.transform.DOScaleY(1, openingYScaleDuration)).SetEase(openingEasingCurve);
            
            sequence.Append(questCompleteCanvasGroup.transform.DOScaleX(1, openingXScaleDuration)).SetEase(openingEasingCurve);
            
            //Show Quest Completed Text
            sequence.Append(questCompleteTitleText.DOFade(1, 0.3f));
            sequence.Join(questCompleteNameText.DOFade(1, 0.2f));
            
            sequence.AppendInterval(2f);

            sequence.Append(questCompleteRewardText.DOFade(1, 0.2f));
            sequence.Append(questRewardCanvasGroup.DOFade(1, 0.2f));
            
            //Wait
            sequence.AppendInterval(5f);
            
            //Remove Quest Completed BG
            sequence.Append(questCompleteCanvasGroup.transform.DOScaleX(0.01f, endingXScaleDuration)).SetEase(endingEasingCurve);
            
            sequence.Append(questCompleteCanvasGroup.transform.DOScaleY(0, endingYScaleDuration)).SetEase(endingEasingCurve);
            sequence.Join(questCompleteCanvasGroup.DOFade(0, endingDuration));
            
            sequence.Play();
            
        }
    }
}