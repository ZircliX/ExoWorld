using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Audios
{
    public class SubtitlesUi : MonoBehaviour
    {
        [field : SerializeField] public TMP_Text NameText { get; private set; }
        [field : SerializeField] public TMP_Text SubtitleText { get; private set; }
        [field : SerializeField] public CanvasGroup SubtitleCanvasGroup { get; private set; }
        [field : SerializeField] public float FadeDuration { get; private set; }

        private List<string> subtitlesTexts = new();
        private float lifeTime;
        private Sequence openSimpleSequence;
        private Sequence closeSimpleSequence;
        private Sequence openMultipleSequence;
        private Sequence closeMultipleSequence;
        
        public void Start()
        {
            closeSimpleSequence = DOTween.Sequence();
            closeSimpleSequence.Append(SubtitleCanvasGroup.DOFade(0, FadeDuration));
            closeSimpleSequence.Join(NameText.DOFade(0, FadeDuration));
            closeSimpleSequence.Join(SubtitleText.DOFade(0, FadeDuration));
            
        }

        public void Initialize(string characterName, List<string> subtitle, SubtitlesManager.SubtitlesUiType uiType, float lifeTime)
        {
            this.lifeTime = lifeTime;
            NameText.text = characterName;
            
            switch (uiType)
            {
                case SubtitlesManager.SubtitlesUiType.Simple:
                    subtitlesTexts = subtitle;
                    SubtitleText.text = subtitlesTexts[0];
                    return;
                
                case SubtitlesManager.SubtitlesUiType.Multiple:
                    
                    return;
            }
        }

        public void DisplaySubtitle()
        {
            openSimpleSequence = DOTween.Sequence();
            openSimpleSequence.Append(SubtitleCanvasGroup.DOFade(1, FadeDuration));
            openSimpleSequence.Join(NameText.DOFade(1, FadeDuration));
            openSimpleSequence.Join(SubtitleText.DOFade(1, FadeDuration));
            openSimpleSequence.Play();
        }


        
        
    }
}