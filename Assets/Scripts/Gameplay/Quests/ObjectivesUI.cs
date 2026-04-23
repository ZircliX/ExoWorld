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
        [SerializeField] private Transform objectiveNameParent;
        [SerializeField] private TMP_Text objectiveProgressText;
        [SerializeField] private Image objectiveIcon;
        [SerializeField] private float lerpTime;
        [SerializeField] private float stepDelay;
        [SerializeField] private float slideOffset;
        
        [Space]
        [SerializeField] private CanvasGroup questCompleteContainer;
        [SerializeField] private TMP_Text questCompleteNameText;
        [SerializeField] private Image questCompleteRewardImage;
        [SerializeField] private TMP_Text questCompleteRewardText;

        private int previousStep;
        private IObjectiveHandler currentObjectiveHandler;

        private float nameOriginX;
        private float textOriginX;
        private float iconOriginX;

        private void Start()
        {
            if (ObjectivesManager.ActiveObjectives.Count == 0) 
                return;
            
            IObjectiveHandler objectiveHandler = ObjectivesManager.ActiveObjectives[0];
            currentObjectiveHandler = objectiveHandler;
            
            currentObjectiveHandler.OnObjectiveProgress += OnObjectiveProgress;
            currentObjectiveHandler.OnObjectiveStateChanged += OnStateChanged;
            currentObjectiveHandler.OnObjectiveStepChanged += OnStepChanged;
            
            // Position d'origine
            nameOriginX = ((RectTransform)objectiveNameParent.transform).anchoredPosition.x;
            textOriginX = ((RectTransform)objectiveProgressText.transform).anchoredPosition.x;
            iconOriginX = ((RectTransform)objectiveIcon.transform).anchoredPosition.x;
            
            // Décale pour l'animation
            ((RectTransform)objectiveNameParent.transform).anchoredPosition = new Vector2(nameOriginX + slideOffset, ((RectTransform)objectiveNameText.transform).anchoredPosition.y);
            ((RectTransform)objectiveProgressText.transform).anchoredPosition = new Vector2(textOriginX + slideOffset, ((RectTransform)objectiveProgressText.transform).anchoredPosition.y);
            ((RectTransform)objectiveIcon.transform).anchoredPosition = new Vector2(iconOriginX + slideOffset, ((RectTransform)objectiveIcon.transform).anchoredPosition.y);
            
            UpdateObjectiveUI(true);
        }

        private void OnDestroy()
        {
            if (currentObjectiveHandler != null)
            {
                currentObjectiveHandler.OnObjectiveProgress -= OnObjectiveProgress;
                currentObjectiveHandler.OnObjectiveStateChanged -= OnStateChanged;
                currentObjectiveHandler.OnObjectiveStepChanged -= OnStepChanged;
            }
        }

        private void OnStateChanged(IObjectiveHandler objectiveHandler, ObjectiveState state)
        {
            if (state is ObjectiveState.Disposed &&
                currentObjectiveHandler == objectiveHandler)
            {
                currentObjectiveHandler.OnObjectiveProgress -= OnObjectiveProgress;
                currentObjectiveHandler.OnObjectiveStateChanged -= OnStateChanged;
                currentObjectiveHandler.OnObjectiveStepChanged -= OnStepChanged;
                ClearObjective();
            }
        }
        
        private void OnObjectiveProgress(ObjectiveProgression progression)
        {
            UpdateObjectiveUI(false);
        }

        private void OnStepChanged(int step)
        {
            UpdateObjectiveUI(false);
        }

        private void UpdateObjectiveUI(bool isAnimated)
        {
            objectiveNameText.text = currentObjectiveHandler.ObjectiveData.ObjectiveName;
            
            float current = currentObjectiveHandler.CurrentProgress.currentProgress;
            float target = currentObjectiveHandler.CurrentProgress.targetProgress;
            bool madeProgress = current > 0 && current <= target;
            
            string stepText = currentObjectiveHandler.ObjectiveData.ObjectiveSteps[currentObjectiveHandler.StepIndex];
            string progressText = madeProgress ? $"({current:0}/{target:0})" : string.Empty;
            objectiveProgressText.text = $"{stepText} {progressText}";

            previousStep = currentObjectiveHandler.StepIndex;

            if (isAnimated)
            {
                float delay = 3.5f;
                DOVirtual.DelayedCall(delay, () =>
                {
                    ((RectTransform)objectiveNameParent.transform).DOAnchorPosX(nameOriginX, lerpTime).SetEase(Ease.OutCubic);
                });
                DOVirtual.DelayedCall(stepDelay * 0.5f + delay, () =>
                {
                    ((RectTransform)objectiveProgressText.transform).DOAnchorPosX(textOriginX, lerpTime).SetEase(Ease.OutCubic);
                });
                DOVirtual.DelayedCall(stepDelay + delay, () =>
                {
                    ((RectTransform)objectiveIcon.transform).DOAnchorPosX(iconOriginX, lerpTime).SetEase(Ease.OutCubic);
                });
            }
        }

        private void ClearObjective()
        {
            if (questCompleteRewardImage == null)
            {
                Debug.LogWarning("questCompleteRewardImage was destroyed before ClearObjective!", gameObject);
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
            
            //Show reward
            questCompleteContainer.transform.localScale = new Vector3(0, 1, 1);
            uiSequence.Append(questCompleteContainer.DOFade(1, 1.5f));
            uiSequence.Join(questCompleteContainer.transform.DOScaleY(1, 0.5f));
            uiSequence.Join(questCompleteContainer.transform.DOScaleX(1, 0.5f));
            
            //Wait
            uiSequence.AppendInterval(3f);
            
            //Remove reward
            uiSequence.Append(questCompleteContainer.DOFade(0, 1.5f));
            uiSequence.Join(questCompleteContainer.transform.DOScaleX(0, 0.5f));
            
            //Play
            uiSequence.Play();

            currentObjectiveHandler = null;
            previousStep = 0;
        }
    }
}