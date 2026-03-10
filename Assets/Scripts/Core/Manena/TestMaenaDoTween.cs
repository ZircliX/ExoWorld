using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Manena
{
    public class TestMaenaDoTween : MonoBehaviour
    {
        [SerializeField] private CanvasGroup questCompleteCanvasGroup;
        [SerializeField] private CanvasGroup questRewardCanvasGroup;
        [SerializeField] private TMP_Text questCompleteTitleText;
        [SerializeField] private TMP_Text questCompleteNameText;
        [SerializeField] private TMP_Text questCompleteRewardText;
        [SerializeField] private Ease easingCurve;
        
        [SerializeField, Range(0,1)] private float openingXScaleDuration;
        [SerializeField, Range(0,1)] private float openingYScaleDuration;
        [SerializeField] private float openingDuration;


        public void DoAnimation()
        {
            Sequence sequence = DOTween.Sequence();
            questCompleteCanvasGroup.transform.localScale = new Vector3(0.01f, 0, 1);
            
            sequence.Append(questCompleteCanvasGroup.DOFade(1, openingDuration));
            sequence.Join(questCompleteCanvasGroup.transform.DOScaleY(1, openingYScaleDuration)).SetEase(easingCurve);
            
            sequence.Append(questCompleteCanvasGroup.transform.DOScaleX(1, openingXScaleDuration)).SetEase(easingCurve);
            
            sequence.Append(questCompleteTitleText.DOFade(1, 0.1f));
            sequence.Join(questCompleteNameText.DOFade(1, 0.1f));
            sequence.AppendInterval(3f);

            sequence.Append(questCompleteRewardText.DOFade(1, 0.1f));
            sequence.Append(questRewardCanvasGroup.DOFade(1, 0.1f));
            
            sequence.Play();
            
        }
    }
}