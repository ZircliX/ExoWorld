using System;
using System.Collections.Generic;
using DG.Tweening;
using OverBang.Pooling;
using TMPro;
using Unity.Services.Authentication.Components;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Audios
{
    public class SubtitlesUi : MonoBehaviour
    {
        [field : SerializeField] public CanvasGroup SubtitleCanvasGroup { get; private set; }
        [field : SerializeField] public TMP_Text NameText { get; private set; }
        [field : SerializeField] public TMP_Text SubtitleText { get; private set; }
        [field : SerializeField] public float FadeDuration { get; private set; }

        public event Action<SubtitlesUi> OnSubtitleBeingKilled;
        
        private List<string> Lines = new();
        private float lifeTime;
        private float timebetween;
        private SubtitlesManager.SubtitlesUiType type;
        
        private Sequence openSequence;
        private Sequence closeSequence;
        
        private Sequence openInternalSequence;
        private Sequence closeInternalSequence;
        
        public void InitializeTweens()
        {
            openSequence = DOTween.Sequence()
                .Append(SubtitleCanvasGroup.DOFade(1, FadeDuration))
                .Join(NameText.DOFade(1, FadeDuration))
                .Join(SubtitleText.DOFade(1, FadeDuration))
                .SetAutoKill(false)
                .Pause();
            
            closeSequence = DOTween.Sequence()
                .Append(SubtitleCanvasGroup.DOFade(0, FadeDuration))
                .Join(NameText.DOFade(0, FadeDuration))
                .Join(SubtitleText.DOFade(0, FadeDuration))
                .SetAutoKill(false)
                .Pause();
            
            closeInternalSequence = DOTween.Sequence()
                .Append(SubtitleText.DOFade(0, 0.2f))
                .SetAutoKill(false)
                .Pause();
            
            openInternalSequence = DOTween.Sequence()
                .Append(SubtitleText.DOFade(1, 0.2f))
                .AppendInterval(timebetween)
                .SetAutoKill(false)
                .Pause();
        }

        public void Initialize(string characterName, List<string> subtitles, SubtitlesManager.SubtitlesUiType uiType, float subtitleLifetime, float timeBetweenLines)
        {
            
            time = 0f;
            Lines = subtitles;
            timebetween = timeBetweenLines;
            lifeTime = subtitleLifetime;
            NameText.text = characterName; 
            type = uiType;
            InitializeTweens();
            
            switch (type)
            {
                case SubtitlesManager.SubtitlesUiType.Simple:
                    
                    SubtitleText.text = Lines[0];
                    openSequence.Restart();
                    
                    return;
                
                case SubtitlesManager.SubtitlesUiType.Multiple:
                    
                    SubtitleText.text = Lines[0];
                    openSequence.Restart();
                    DisplayMultipleSubtitles();
                    
                    return;
            }
        }

        private float time;
        public void Tick(float deltaTime)
        {
            time += deltaTime;
            if (time >= lifeTime)
            {
                KillSubtitle();
            }
        }

        private int i;
        private void DisplayMultipleSubtitles()
        {
            i++;
            if (i > Lines.Count) return;
            
            closeInternalSequence.Restart();
            SubtitleText.text = Lines[i];
            openInternalSequence.Play().OnComplete(DisplayMultipleSubtitles);
        }
        
        private void KillSubtitle()
        {
            OnSubtitleBeingKilled?.Invoke(this);
            closeSequence.Play().OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}