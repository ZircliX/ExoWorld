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
        
        [SerializeField, Space] private Vector2 outVec, inVec, minVec;
        
        [SerializeField, Range(0, 3)] private float loopDuration = 1.5f;
        [SerializeField, Range(0, 3)] private float loopCooldown = 0.5f;
        [SerializeField] private Ease interactEasingCurve;
        [SerializeField] private Color interactableColor;

        [SerializeField, Space] private DetectionArea enterArea;
        [SerializeField] private DetectionArea interactArea;

        private Sequence sequence;
        private Sequence interactSequence;
        
        private void Awake()
        {
            waypointIconOutline.transform.localScale = new Vector3(0, 0, 0);
            waypointIcon.transform.localScale = new Vector3(0, 0, 0);
            
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
            sequence = DOTween.Sequence();
            Sequence loop = DOTween.Sequence();
            Image iconOutline = waypointIconOutline.GetComponent<Image>();
            
            loop.Append(waypointIconOutline.DOSizeDelta(outVec, loopDuration)).SetEase(interactEasingCurve).SetLoops(-1, LoopType.Restart);
            loop.Append(waypointIconOutline.DOSizeDelta (inVec, loopDuration)).SetEase(interactEasingCurve).SetLoops(-1, LoopType.Restart);
            loop.Pause();

            //Waypoint Slow Bounce
            Debug.Log("gdrikfhuirodgdiughdfgondfgondgonndgondfgoindfgondfgodfngo");
            sequence.Append(waypointCanvasGroup.DOFade(1, 1f)).SetEase(interactEasingCurve);
            sequence.Join(waypointCanvasGroup.transform.DOScale(1, 1f)).SetEase(interactEasingCurve);
            sequence.Join(iconOutline.DOColor(Color.white, 0.5f)).SetEase(interactEasingCurve);

            sequence.AppendInterval(loopCooldown);
            sequence.Append(loop);
            
            sequence.Play();
        }
        
        private void OnEnterExit(Collider other, object target)
        {
            sequence.Kill();
        }
        
        private void OnInteractEnter(Collider other, object target)
        {
            sequence.Kill();
            
            Sequence seq = DOTween.Sequence();
            Image iconOutline = waypointIconOutline.GetComponent<Image>();

            seq.Append(iconOutline.DOColor(interactableColor, 0.5f)).SetEase(interactEasingCurve);
            seq.Join(waypointIconOutline.DOSizeDelta(minVec, loopDuration)).SetEase(interactEasingCurve);
            seq.Append(waypointIconOutline.DOSizeDelta(inVec, loopDuration)).SetEase(interactEasingCurve);
        }
        
        private void OnInteractExit(Collider other, object target)
        {
            OnEnterEnter(other, target);
            interactSequence.Kill();
        }
    }
}