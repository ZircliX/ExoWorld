using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class ObjectivesUI : ZTools.ObjectiveSystem.Core.ObjectivesUI
    {
        [SerializeField] private CanvasGroup questContainer;
        [SerializeField] private TMP_Text objectiveNameText;
        [SerializeField] private TMP_Text objectiveProgressText;
        
        [Space]
        [SerializeField] private CanvasGroup questCompleteContainer;
        [SerializeField] private TMP_Text questCompleteNameText;
        [SerializeField] private Image questCompleteRewardImage;
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
            if (state is ObjectiveState.Completed or ObjectiveState.Disposed)
            {
                objectiveHandler.OnObjectiveStateChanged += OnStateChanged;
                ClearObjective();
            }
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
            Debug.Log($"{current}/{target}");
            bool madeProgress = current > 0 && current < target;
            
            string stepText = objectiveHandler.ObjectiveData.ObjectiveSteps[objectiveHandler.StepIndex];
            string progressText = madeProgress ? $"({current:0}/{target:0})" : string.Empty;
            objectiveProgressText.text = $"{stepText} {progressText}";

            previousStep = objectiveHandler.StepIndex;
        }

        protected override void ClearObjective()
        {
            Sequence uiSequence = DOTween.Sequence();

            //Remove quest
            uiSequence.Append(questContainer.DOFade(0, 0.2f).OnComplete(() =>
            {
                objectiveNameText.text = string.Empty;
                objectiveProgressText.text = string.Empty;
            }));
            
            questCompleteNameText.text = currentObjectiveHandler.ObjectiveData.ObjectiveName;
                
            if (currentObjectiveHandler.ObjectiveData is ObjectiveDataQuest { Reward: TrinititeRewardData trinititeRewardData } questData)
            {
                questCompleteRewardText.text = $"{trinititeRewardData.TrinititeData.ItemName} : {trinititeRewardData.TrinititeData.Quantity}";
                questCompleteRewardImage.sprite = trinititeRewardData.TrinititeData.Icon;
            }
            
            //Show reward + initialize texts
            uiSequence.Append(questCompleteContainer.DOFade(1, 1.5f));
            
            //Wait
            uiSequence.AppendInterval(3f);
            
            //Remove reward
            uiSequence.Append(questCompleteContainer.DOFade(0, 1.5f));
            
            //Play
            uiSequence.Play();

            currentObjectiveHandler = null;
            previousStep = 0;
        }
    }
}