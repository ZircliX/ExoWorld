using DG.Tweening;
using TMPro;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace ZTools.ObjectiveSystem.Sample
{
    public class ObjectivesUI : Core.ObjectivesUI
    {
        [SerializeField] private TMP_Text objectiveNameText;
        [SerializeField] private TMP_Text objectiveProgressText;

        private int previousStep;
        private IObjectiveHandler currentObjectiveHandler;

        private void Start()
        {
            IObjectiveHandler objectiveHandler = ObjectivesManager.ActiveObjectives[0];
            
            currentObjectiveHandler = objectiveHandler;
            objectiveHandler.OnObjectiveStepChanged += OnStepChanged;
            UpdateObjectiveUI(objectiveHandler);
        }

        protected override void OnObjectiveChanged(IObjectiveHandler objectiveHandler)
        {
            if (objectiveHandler == null)
            {
                ClearObjective();
            }
        }

        private void OnStepChanged(int obj)
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
            float progress = Mathf.Abs(current - target);
            
            string stepText = objectiveHandler.ObjectiveData.ObjectiveSteps[objectiveHandler.StepIndex];
            string progressText = progress <= 0 ? string.Empty : $"({current:0}/{target:0})";
            objectiveProgressText.text = $"{stepText} {progressText}";

            previousStep = objectiveHandler.StepIndex;
        }

        protected override void ClearObjective()
        {
            objectiveNameText.text = string.Empty;
            objectiveProgressText.text = string.Empty;
        }
    }
}