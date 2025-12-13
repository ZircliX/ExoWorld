using DG.Tweening;
using TMPro;
using UnityEngine;
using ZTools.ObjectiveSystem.Core;

namespace ZTools.ObjectiveSystem.Sample
{
    public class ObjectivesUI : Core.ObjectivesUI
    {
        [SerializeField] private TMP_Text objectiveNameText;
        [SerializeField] private TMP_Text objectiveDescriptionText;
        [SerializeField] private TMP_Text objectiveStepText;
        [SerializeField] private TMP_Text objectiveProgressText;

        private int previousStep;

        protected override void OnObjectiveChanged(IObjectiveHandler objectiveHandler)
        {
            if (objectiveHandler == default)
            {
                ClearObjective();
                return;
            }
            
            UpdateObjectiveUI(objectiveHandler);
        }

        protected override void UpdateObjectiveUI(IObjectiveHandler objectiveHandler)
        {
            objectiveNameText.text = objectiveHandler.ObjectiveData.ObjectiveName;
            
            if (objectiveDescriptionText != null)
            {
                objectiveDescriptionText.text = objectiveHandler.ObjectiveData.ObjectiveDescription;
            }
            
            objectiveStepText.text = objectiveHandler.ObjectiveData.ObjectiveSteps[objectiveHandler.StepIndex];
            if (objectiveHandler.StepIndex != previousStep)
            {
                objectiveStepText.DOColor(Color.orange, 0.2f).OnComplete(() =>
                {
                    objectiveStepText.DOColor(Color.white, 1.5f);
                });
            }
            
            float current = objectiveHandler.CurrentProgress.currentProgress;
            float target = objectiveHandler.CurrentProgress.targetProgress;
            float progress = Mathf.Abs(current - target);
            objectiveProgressText.text = progress <= 0 ? string.Empty : $"{progress:0.0}s";

            previousStep = objectiveHandler.StepIndex;
        }

        protected override void ClearObjective()
        {
            objectiveNameText.text = string.Empty;
            
            if (objectiveDescriptionText != null)
            {
                objectiveDescriptionText.text = string.Empty;
            }

            objectiveProgressText.text = string.Empty;
        }
    }
}