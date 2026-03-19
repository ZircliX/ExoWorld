using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Audios
{
    public class SubtitlesUi : MonoBehaviour
    {
        [field : SerializeField] public CanvasGroup SubtitleCanvasGroup { get; private set; }
        [field : SerializeField] public TMP_Text NameText { get; private set; }
        [field : SerializeField] public TMP_Text SubtitleText { get; private set; }
        [field: SerializeField] public float FadeDuration { get; private set; } = 0.5f;
        public bool IsDead { get; private set; }
        public event Action<SubtitlesUi> OnSubtitleBeingKilled;
        
        private List<string> lines = new List<string>();
        private float lifeTime;
        private float timeBetween;
        private SubtitlesManager.SubtitlesUiType type;
        
        private Sequence openSequence;
        private Sequence closeSequence;
        private Sequence showTextSequence;
        
        private Sequence intervalSequence;
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
            
            closeInternalSequence = DOTween.Sequence()
                .Append(SubtitleText.DOFade(0, 0.2f))
                .SetAutoKill(false)
                .Pause();
            
            intervalSequence = DOTween.Sequence()
                .Append(SubtitleText.DOFade(1, 0.5f))
                .AppendInterval(timeBetween)
                .AppendCallback(() => closeInternalSequence.Restart())
                .OnComplete(DisplayMultipleSubtitles)
                .SetAutoKill(false)
                .Pause();
        }

        public void Initialize(string characterName, List<string> subtitles, SubtitlesManager.SubtitlesUiType uiType, float subtitleLifetime, float timeBetweenLines, Color color)
        {
            IsDead = false;
            time = 0f;
            lines = subtitles;
            timeBetween = timeBetweenLines;
            lifeTime = subtitleLifetime;
            NameText.text = characterName; 
            NameText.color = color; 
            type = uiType;
            
            InitializeTweens();
            
            switch (type)
            {
                case SubtitlesManager.SubtitlesUiType.Simple:
                    
                    SubtitleText.text = lines[0];
                    openSequence.Restart();
                    showTextSequence.Restart();
                    
                    return;
                
                case SubtitlesManager.SubtitlesUiType.Multiple:
                    
                    SubtitleText.text = lines[0];
                    openSequence.Restart();
                    showTextSequence.Play().AppendInterval(timeBetween).OnComplete(DisplayMultipleSubtitles);
                    
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
            if (i > lines.Count)
            {
                closeSequence.Play();
                return;
            }
            
            SubtitleText.text = lines[i];
            intervalSequence.Play();
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