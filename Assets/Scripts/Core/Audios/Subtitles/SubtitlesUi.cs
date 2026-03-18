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
        [field: SerializeField] public float FadeDuration { get; private set; } = 0.1f;
        public bool IsDead { get; private set; }
        public event Action<SubtitlesUi> OnSubtitleBeingKilled;
        
        private List<string> Lines = new();
        private float lifeTime;
        private float timebetween;
        private SubtitlesManager.SubtitlesUiType type;
        
        private Sequence openSequence;
        private Sequence closeSequence;
        private Sequence showTextSequence;
        
        private Sequence IntervalSequence;
        private Sequence closeInternalSequence;
        
        public void InitializeTweens()
        {
            openSequence = DOTween.Sequence()
                .Append(SubtitleCanvasGroup.DOFade(1, FadeDuration))
                .SetAutoKill(false)
                .Pause();

            showTextSequence = DOTween.Sequence()
                .Append(SubtitleText.DOFade(1, 0.1f))
                .SetAutoKill(false)
                .Pause();
            
            closeSequence = DOTween.Sequence()
                .Append(SubtitleCanvasGroup.DOFade(0, FadeDuration))
                .Join(NameText.DOFade(0, FadeDuration))
                .Join(SubtitleText.DOFade(0, FadeDuration))
                .SetAutoKill(false)
                .Pause();
            
            IntervalSequence = DOTween.Sequence()
                .Append(SubtitleText.DOFade(1, 0.1f))
                .AppendInterval(timebetween)
                .Append(closeInternalSequence)
                .SetAutoKill(false)
                .Pause();

            closeInternalSequence = DOTween.Sequence()
                .Append(SubtitleText.DOFade(0, 0.2f))
                .SetAutoKill(false)
                .Pause();
        }

        public void Initialize(string characterName, List<string> subtitles, SubtitlesManager.SubtitlesUiType uiType, float subtitleLifetime, float timeBetweenLines, Color color)
        {
            IsDead = false;
            time = 0f;
            Lines = subtitles;
            timebetween = timeBetweenLines;
            lifeTime = subtitleLifetime;
            NameText.text = characterName; 
            NameText.color = color; 
            type = uiType;
            
            InitializeTweens();
            
            switch (type)
            {
                case SubtitlesManager.SubtitlesUiType.Simple:
                    
                    SubtitleText.text = Lines[0];
                    openSequence.Restart();
                    showTextSequence.Restart();
                    
                    return;
                
                case SubtitlesManager.SubtitlesUiType.Multiple:
                    
                    SubtitleText.text = Lines[0];
                    openSequence.Restart();
                    IntervalSequence.Play().OnComplete(DisplayMultipleSubtitles);
                    
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
            if (IsDead) return;
            i++;
            if (i > Lines.Count)
            {
                closeSequence.Play();
                return;
            }
            
            SubtitleText.text = Lines[i];
            IntervalSequence.Play().OnComplete(DisplayMultipleSubtitles);
        }
        
        private void KillSubtitle()
        {
            IsDead = true;
            OnSubtitleBeingKilled?.Invoke(this);
            closeSequence.Play().OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }
    }
}