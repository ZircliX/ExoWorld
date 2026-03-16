using System;
using OverBang.ExoWorld.Gameplay.Core.Menus;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core.Menus
{
    public class PauseMenuUI : NavigablePanel
    {
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button menuButton;
        
        [SerializeField, ReadOnly] private InputAction escapeAction;

        public event Action OnEscapeClicked;
        public event Action OnResumeClicked;
        public event Action OnMenuClicked;
        
        private void OnEnable()
        {
            resumeButton.onClick.AddListener(() => OnResumeClicked?.Invoke());
            backButton.onClick.AddListener(() => OnResumeClicked?.Invoke());
            menuButton.onClick.AddListener(() => OnMenuClicked?.Invoke());
            
            escapeAction = new InputAction(binding: "<Keyboard>/escape");
            escapeAction.performed += BackClicked;
            escapeAction.Enable();
        }

        private void OnDisable()
        {
            resumeButton.onClick.RemoveListener(() => OnResumeClicked?.Invoke());
            backButton.onClick.RemoveListener(() => OnResumeClicked?.Invoke());
            menuButton.onClick.RemoveListener(() => OnMenuClicked?.Invoke());
            
            escapeAction.performed -= BackClicked;
            escapeAction.Disable();
            escapeAction.Dispose();
        }

        private void BackClicked(InputAction.CallbackContext context)
        {
            OnEscapeClicked?.Invoke();
        }

        protected override void OnShow()
        {
            HUD.Instance.ChangeHudState(true);
        }
        
        protected override void OnHide()
        {
            HUD.Instance.ChangeHudState(false);
        }
    }
}