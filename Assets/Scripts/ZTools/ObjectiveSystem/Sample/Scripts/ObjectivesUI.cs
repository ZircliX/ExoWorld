using TMPro;
using UnityEngine;
using ZTools.ObjectiveSystem.Core.Interfaces;

namespace ZTools.ObjectiveSystem.Sample
{
    public class ObjectivesUI : Core.ObjectivesUI
    {
        [SerializeField] private TMP_Text objectiveNameText;
        [SerializeField] private TMP_Text objectiveDescriptionText;
        [SerializeField] private TMP_Text objectiveProgressText;

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
            objectiveDescriptionText.text = objectiveHandler.ObjectiveData.ObjectiveDescription;
            objectiveProgressText.text = objectiveHandler.CurrentProgress.ToString();
        }

        protected override void ClearObjective()
        {
            objectiveNameText.text = string.Empty;
            objectiveDescriptionText.text = string.Empty;
            objectiveProgressText.text = string.Empty;
        }
    }
}