using System.Collections.Generic;
using OverBang.ExoWorld.Core.Audios.ContextualDialogues;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Database;
using OverBang.ExoWorld.Core.Settings;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Audios
{
    public class SubtitlesManager : MonoBehaviour
    {
        [field: SerializeField] public bool Enabled { get; private set; } = true; 
        [SerializeField] private int maxSubtitlesTextLenght;
        [SerializeField] private RectTransform subtitleArea;
        [SerializeField] private RectTransform subtitlePrefab;
        
        private List<SubtitlesUi> subtitlesUisQueue;
        private HashSet<CdQueued> displayedDialogues = new HashSet<CdQueued>();
        
        public void Awake()
        {
            subtitlesUisQueue = new List<SubtitlesUi>();
            displayedDialogues = new HashSet<CdQueued>();
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
            float dt = Time.deltaTime;
            for (int i = subtitlesUisQueue.Count - 1; i >= 0; i--)
            {
                subtitlesUisQueue[i].Tick(dt);
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
            
            LayoutRebuilder.ForceRebuildLayoutImmediate(subtitleArea);
        }

        public void RemoveSubtitleUi(SubtitlesUi ui)
        {
            subtitlesUisQueue.Remove(ui);
            LayoutRebuilder.ForceRebuildLayoutImmediate(subtitleArea);
        }

        public void DisplaySubtitle(CdQueued cdQueued)
        {
            if (displayedDialogues.Contains(cdQueued)) return;
            if (!GameDatabase.Global.TryGetAssetByID(cdQueued.context.characterDataId, out CharacterData characterData)) return;

            displayedDialogues.Add(cdQueued);

            RectTransform subtitle = Instantiate(subtitlePrefab, subtitleArea.transform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(subtitle);
            subtitle.TryGetComponent(out SubtitlesUi subtitlesUi);

            subtitlesUi.OnSubtitleBeingKilled += _ => displayedDialogues.Remove(cdQueued);
            AddSubtitleUi(subtitlesUi);

            List<string> lines = SmartSubtitleWrapper.Split(cdQueued.dialogue.text, maxSubtitlesTextLenght);
            SubtitlesUiType uiType = lines.Count > 1 ? SubtitlesUiType.Multiple : SubtitlesUiType.Simple;
            subtitlesUi.Initialize(characterData.Name, lines, uiType, cdQueued.dialogue.subtitleLifetime, cdQueued.dialogue.timeBetweenLines, characterData.Color);
        }

        public enum SubtitlesUiType
        {
            Simple,
            Multiple
        }
    }
}