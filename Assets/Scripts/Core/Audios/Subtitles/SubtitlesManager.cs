using System.Collections.Generic;
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
        
        /// <summary> For value : true = Subtitles Activated |  false : Subtitles disabled </summary> <param name="value"></param>
        public void SetSubtitlesEnabled(bool value)
        {
            Enabled = value;
        }


        public void AddSubtitleUi(SubtitlesUi ui)
        {
            subtitlesUisQueue.Add(ui);
        }

        public void RemoveSubtitleUi(SubtitlesUi ui)
        {
            subtitlesUisQueue.Remove(ui);
        }

        public void DisplaySubtitle(string characterName, string subtitleText, float subtitleLifetime)
        {
            GameObject subtitle = Instantiate(subtitlePrefab, subtitleArea.transform);
            subtitle.transform.SetParent(subtitleArea.transform);
            subtitle.TryGetComponent(out SubtitlesUi subtitlesUi);
            
            List<string> lines = SmartSubtitleWrapper.Split(subtitleText, maxSubtitlesTextLenght);
            if (lines.Count == 1)
            {
                subtitlesUi.Initialize(characterName, lines, SubtitlesUiType.Simple, subtitleLifetime);
            }
            else if (lines.Count > 1)
            {
                subtitlesUi.Initialize(characterName, lines, SubtitlesUiType.Multiple, subtitleLifetime);
            }
        }
        
        
        public bool CheckTextLength(SubtitlesUi ui)
        {
            return ui.SubtitleText.textInfo.characterCount <= maxSubtitlesTextLenght;
        }

        public enum SubtitlesUiType
        {
            Simple,
            Multiple
        }
    }
}