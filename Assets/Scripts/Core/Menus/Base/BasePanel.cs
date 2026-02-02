using DG.Tweening;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Menus
{
    public abstract class BasePanel : MonoBehaviour, IPanel, ISelectable
    {
        [SerializeField] protected Selectable firstSelectable;
        [SerializeField, Self] protected CanvasPanelGroup canvasGroup;
        protected EventSystem EventSystem { get; private set; }

        public bool IsActive => gameObject.activeSelf;
        public virtual Selectable FirstSelectable => firstSelectable;

        protected virtual void Awake()
        {
            EventSystem = EventSystem.current;
        }

        public virtual void Show()
        {
            canvasGroup.Open();
            OnShow();
            SetInitialFocus();
        }

        public virtual void Hide()
        {
            canvasGroup.Close();
            OnHide();
            ClearFocus();
        }

        protected virtual void SetInitialFocus()
        {
            if (FirstSelectable != null)
                EventSystem.SetSelectedGameObject(FirstSelectable.gameObject);
        }

        protected virtual void ClearFocus()
        {
            if (EventSystem.currentSelectedGameObject?.transform.IsChildOf(transform) ?? false)
                EventSystem.SetSelectedGameObject(null);
        }

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }
        
        protected static Navigation CreateNavigation(Selectable up = null, Selectable down = null, 
            Selectable left = null, Selectable right = null)
        {
            return new Navigation
            {
                mode = Navigation.Mode.Explicit,
                selectOnUp = up,
                selectOnDown = down,
                selectOnLeft = left,
                selectOnRight = right
            };
        }
    }
}