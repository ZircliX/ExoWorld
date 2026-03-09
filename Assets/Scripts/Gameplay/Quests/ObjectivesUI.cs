using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using ZTools.ObjectiveSystem.Core;

namespace OverBang.ExoWorld.Gameplay.Quests
{
    public class ObjectivesUI : MonoBehaviour
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
            currentObjectiveHandler = objectiveHandler;
            currentObjectiveHandler.OnObjectiveProgress += OnObjectiveProgress;
            currentObjectiveHandler.OnObjectiveStateChanged += OnStateChanged;
            currentObjectiveHandler.OnObjectiveStepChanged += OnStepChanged;
            
            UpdateObjectiveUI();
        }

        private void OnObjectiveProgress(ObjectiveProgression progression)
        {
            UpdateObjectiveUI();
        }

        private void OnStateChanged(IObjectiveHandler objectiveHandler, ObjectiveState state)
        {
            if (state is ObjectiveState.Completed &&
                currentObjectiveHandler == objectiveHandler)
            {
                currentObjectiveHandler.OnObjectiveProgress -= OnObjectiveProgress;
                currentObjectiveHandler.OnObjectiveStateChanged -= OnStateChanged;
                currentObjectiveHandler.OnObjectiveStepChanged -= OnStepChanged;
                ClearObjective();
            }
        }

        private void OnStepChanged(int step)
        {
            UpdateObjectiveUI();
        }

        private void UpdateObjectiveUI()
        {
            objectiveNameText.text = currentObjectiveHandler.ObjectiveData.ObjectiveName;
            
            if (currentObjectiveHandler.StepIndex != previousStep)
            {
                objectiveProgressText.DOColor(Color.red, 0.2f).OnComplete(() =>
                {
                    objectiveProgressText.DOColor(Color.white, 1.5f);
                });
            }
            
            float current = currentObjectiveHandler.CurrentProgress.currentProgress;
            float target = currentObjectiveHandler.CurrentProgress.targetProgress;
            bool madeProgress = current > 0 && current <= target;
            
            string stepText = currentObjectiveHandler.ObjectiveData.ObjectiveSteps[currentObjectiveHandler.StepIndex];
            string progressText = madeProgress ? $"({current:0}/{target:0})" : string.Empty;
            objectiveProgressText.text = $"{stepText} {progressText}";

            previousStep = currentObjectiveHandler.StepIndex;
        }

        private void ClearObjective()
        {
            if (questCompleteRewardImage == null)
            {
                Debug.LogWarning("questCompleteRewardImage was destroyed before ClearObjective!");
                //return;
            }
            
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
                questCompleteRewardText.text = $"{trinititeRewardData.TrinititeData.ItemName} : {trinititeRewardData.RewardQuantity}";
                
                //Debug.Log(questCompleteRewardImage);
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