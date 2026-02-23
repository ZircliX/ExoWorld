using DG.Tweening;
using TMPro;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace ZTools.ObjectiveSystem.Sample
{
    public class ObjectivesUISample : Core.ObjectivesUI
    {
        [SerializeField] private CanvasGroup questContainer;
        [SerializeField] private TMP_Text objectiveNameText;
        [SerializeField] private TMP_Text objectiveProgressText;
        
        [Space]
        [SerializeField] private CanvasGroup questCompleteContainer;
        [SerializeField] private TMP_Text questCompleteNameText;
        [SerializeField] private TMP_Text questCompleteRewardText;

        private int previousStep;
        private IObjectiveHandler currentObjectiveHandler;

        private void Start()
        {
            if (ObjectivesManager.ActiveObjectives.Count == 0) 
                return;
            
            IObjectiveHandler objectiveHandler = ObjectivesManager.ActiveObjectives[0];
            objectiveHandler.OnObjectiveStateChanged += OnStateChanged;
            
            currentObjectiveHandler = objectiveHandler;
            objectiveHandler.OnObjectiveStepChanged += OnStepChanged;
            UpdateObjectiveUI(objectiveHandler);
        }

        private void OnStateChanged(IObjectiveHandler objectiveHandler, ObjectiveState state)
        {
            if (currentObjectiveHandler == objectiveHandler && state is ObjectiveState.Completed or ObjectiveState.Disposed)
                ClearObjective();
        }

        private void OnStepChanged(int step)
        {
            UpdateObjectiveUI(currentObjectiveHandler);
        }

        protected override void UpdateObjectiveUI(IObjectiveHandler objectiveHandler)
        {
            objectiveNameText.text = objectiveHandler.ObjectiveData.ObjectiveName;
            
            if (objectiveHandler.StepIndex != previousStep)
            {
                objectiveProgressText.DOColor(Color.red, 0.2f).OnComplete(() =>
                {
                    objectiveProgressText.DOColor(Color.white, 1.5f);
                });
            }
            
            float current = objectiveHandler.CurrentProgress.currentProgress;
            float target = objectiveHandler.CurrentProgress.targetProgress;
            float progress = target - current;
            
            string stepText = objectiveHandler.ObjectiveData.ObjectiveSteps[objectiveHandler.StepIndex];
            string progressText = progress is <= 0 or >= 60 ? string.Empty : $"({current:0}/{target:0})";
            objectiveProgressText.text = $"{stepText} {progressText}";

            previousStep = objectiveHandler.StepIndex;
        }

        protected override void ClearObjective()
        {
            questCompleteContainer.DOFade(1, 0.5f).OnComplete(() =>
            {
                // Remove Quest ui
                questContainer.DOFade(0, 0.2f).OnComplete(() =>
                {
                    objectiveNameText.text = string.Empty;
                    objectiveProgressText.text = string.Empty;
                });
            });
        }
    }
}