using System;
using DG.Tweening;
using OverBang.ExoWorld.Core.Components;
using UnityEngine;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Manena
{
    public class InteractableWaypoint : MonoBehaviour
    {
        [SerializeField] private CanvasGroup waypointCanvasGroup;
        [SerializeField] private RectTransform waypointIconOutline;
        [SerializeField] private RectTransform waypointIcon;
        [SerializeField] private RectTransform parentRect;
        
        [SerializeField, Space] private Vector2 outVec, inVec, minVec;
        
        [SerializeField, Range(0, 3)] private float loopDuration = 1.5f;
        [SerializeField, Range(0, 3)] private float loopCooldown = 0.5f;
        [SerializeField] private Ease scaleInEasingCurve;
        [SerializeField] private Ease scaleOutEasingCurve;
        [SerializeField] private Ease interactEasingCurve;
        [SerializeField] private Ease loopInEasingCurve;
        [SerializeField] private Ease loopOutEasingCurve;
        [SerializeField] private Color interactableColor;

        [SerializeField, Space] private DetectionArea enterArea;
        [SerializeField] private DetectionArea interactArea;

        private Sequence sequence;
        private Sequence interactSequence;
        
        private Transform player;
        
        private void Awake()
        {
            parentRect.localScale = new Vector3(0, 0, 0);
            
            enterArea.SetAllowedTags("LocalPlayer");
            interactArea.SetAllowedTags("LocalPlayer");
        }

        private void OnEnable()
        {
            enterArea.OnEnter += OnEnterEnter;
            enterArea.OnExit += OnEnterExit;
            
            interactArea.OnEnter += OnInteractEnter;
            interactArea.OnExit += OnInteractExit;
        }

        private void OnDisable()
        {
            enterArea.OnEnter -= OnEnterEnter;
            enterArea.OnExit -= OnEnterExit;
            
            interactArea.OnEnter -= OnInteractEnter;
            interactArea.OnExit -= OnInteractExit;
        }

        private void OnEnterEnter(Collider other, object target)
        {
            player = other.transform;
            
            sequence?.Kill();
            sequence = DOTween.Sequence();
            Image iconOutline = waypointIconOutline.GetComponent<Image>();

            // Intro
            sequence.Append(parentRect.DOScale(1, 1f).SetEase(scaleInEasingCurve));
            sequence.Join(waypointCanvasGroup.DOFade(1, 0.5f).SetEase(interactEasingCurve));
            sequence.Join(iconOutline.DOColor(Color.white, 0.5f).SetEase(interactEasingCurve));
            
            //sequence.AppendInterval(loopCooldown);

            // Loop part as a callback that starts a separate looping sequence
            sequence.OnComplete(() =>
            {
                Sequence loop = DOTween.Sequence();
                loop.Append(waypointIconOutline.DOSizeDelta(outVec, loopDuration).SetEase(loopOutEasingCurve));
                loop.Append(waypointIconOutline.DOSizeDelta(inVec, loopDuration).SetEase(loopInEasingCurve));
                loop.SetLoops(-1, LoopType.Restart);
        
                sequence = loop;
            });

            sequence.Play();
        }
        
        private void OnEnterExit(Collider other, object target)
        {
            player = null;
            
            sequence.Kill();
            sequence.Append(parentRect.DOScale(0, 1f).SetEase(scaleOutEasingCurve));
            waypointCanvasGroup.DOFade(0, 0.5f).SetEase(scaleOutEasingCurve).OnComplete(() =>
            {
                parentRect.localScale = new Vector3(0, 0, 0);
            });
        }
        
        private void OnInteractEnter(Collider other, object target)
        {
            sequence.Kill();
            
            Sequence seq = DOTween.Sequence();
            Image iconOutline = waypointIconOutline.GetComponent<Image>();

            //parentRect.DOScale(0.8f, 1f).SetEase(interactEasingCurve);
            seq.Append(iconOutline.DOColor(interactableColor, 0.1f)).SetEase(interactEasingCurve);
            seq.Join(waypointIconOutline.DOSizeDelta(minVec, 0.5f)).SetEase(loopOutEasingCurve);
            seq.Append(waypointIconOutline.DOSizeDelta(inVec, 0.2f)).SetEase(loopInEasingCurve);
        }
        
        private void OnInteractExit(Collider other, object target)
        {
            Image iconOutline = waypointIconOutline.GetComponent<Image>();
            //iconOutline.DOColor(Color.white, 0.5f).SetEase(interactEasingCurve);
            
            OnEnterEnter(other, target);
        }

        private void LateUpdate()
        {
            if (player == null) return;
            
            transform.rotation = Quaternion.LookRotation(transform.position - player.position);
        }
    }
}