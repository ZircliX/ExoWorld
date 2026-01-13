using System;
using InterfaceAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OverBang.GameName.Core.Menus
{
    public abstract class BaseSelector<T> : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        [SerializeField] protected TMP_Text displayText;
        [SerializeField] protected Button leftButton;
        [SerializeField] protected Button rightButton;
        [SerializeField] protected InterfaceReference<IInputProvider> inputProvider;

        protected bool isFocused;

        public event Action<T> OnValueChanged;
        public abstract T CurrentValue { get; }

        protected virtual void Awake()
        {
            leftButton.onClick.AddListener(OnPreviousClicked);
            rightButton.onClick.AddListener(OnNextClicked);
        }

        protected virtual void Start()
        {
            if (inputProvider == null)
                return;
            
            inputProvider.Value.NavigationAction.performed += OnNavigationInput;
        }

        protected virtual void OnDestroy()
        {
            if (inputProvider == null)
                return;
            
            inputProvider.Value.NavigationAction.performed -= OnNavigationInput;
        }

        public void OnSelect(BaseEventData eventData) => isFocused = true;
        public void OnDeselect(BaseEventData eventData) => isFocused = false;

        protected virtual void OnNavigationInput(InputAction.CallbackContext context)
        {
            if (!isFocused) return;

            Vector2 input = context.ReadValue<Vector2>();
            
            switch (input.x)
            {
                case > 0.5f:
                    OnNextClicked();
                    break;
                case < -0.5f:
                    OnPreviousClicked();
                    break;
            }
        }

        protected abstract void OnNextClicked();
        protected abstract void OnPreviousClicked();
        protected abstract void UpdateDisplay();

        protected void InvokeValueChanged()
        {
            OnValueChanged?.Invoke(CurrentValue);
        }
    }
}