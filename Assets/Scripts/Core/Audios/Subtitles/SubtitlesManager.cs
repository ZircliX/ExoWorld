using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Audios.ContextualDialogues;
using OverBang.ExoWorld.Core.Settings;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Audios
{
    public class SubtitlesManager : MonoBehaviour
    {
        [field : SerializeField] public bool Enabled { get; private set; } 
        [SerializeField] private int maxSubtitlesTextLenght;
        [SerializeField] private GameObject subtitleArea;
        [SerializeField] private GameObject subtitlePrefab;
        
        private List<SubtitlesUi> subtitlesUisQueue;
        
        public void Start()
        {
            subtitlesUisQueue = new List<SubtitlesUi>();
        }

        private void OnEnable()
        {
            PlayerSettings.Instance.OnSubtitlesChanged += SetSubtitlesEnabled;
        }

        private void OnDisable()
        {
            PlayerSettings.Instance.OnSubtitlesChanged -= SetSubtitlesEnabled;
        }

        private void Update()
        {
            foreach (SubtitlesUi subtitle in subtitlesUisQueue)
            {
                subtitle.Tick(Time.deltaTime);
            }
        }

        /// <summary> For value : true = Subtitles Activated |  false : Subtitles disabled </summary> <param name="value"></param>
        public void SetSubtitlesEnabled(bool value)
        {
            Enabled = value;
        }
        
        public void AddSubtitleUi(SubtitlesUi ui)
        {
            ui.OnSubtitleBeingKilled += RemoveSubtitleUi;
            subtitlesUisQueue.Add(ui);
        }

        public void RemoveSubtitleUi(SubtitlesUi ui)
        {
            subtitlesUisQueue.Remove(ui);
        }

        public void DisplaySubtitle(CdQueued cdQueued)
        {
            DisplaySubtitle(cdQueued.context.data.Name, cdQueued.dialogue.text, cdQueued.dialogue.subtitleLifetime, cdQueued.dialogue.timeBetweenLines);
        }
        public void DisplaySubtitle(string characterName, string subtitleText, float subtitleLifetime, float  timeBetweenLines)
        {
            GameObject subtitle = Instantiate(subtitlePrefab, subtitleArea.transform);
            subtitle.transform.SetParent(subtitleArea.transform);
            subtitle.TryGetComponent(out SubtitlesUi subtitlesUi);
            AddSubtitleUi(subtitlesUi);
            List<string> lines = SmartSubtitleWrapper.Split(subtitleText, maxSubtitlesTextLenght);

            SubtitlesUiType uiType;
            
            if (lines.Count == 1)
            {
                uiType = SubtitlesUiType.Simple;
            }
            else if (lines.Count > 1)
            {
                uiType = SubtitlesUiType.Multiple;
            }
            
            uiType = SubtitlesUiType.Simple;
            
            subtitlesUi.Initialize(characterName, lines, uiType, subtitleLifetime, timeBetweenLines);
        }

        public enum SubtitlesUiType
        {
            Simple,
            Multiple
        }
    }
}