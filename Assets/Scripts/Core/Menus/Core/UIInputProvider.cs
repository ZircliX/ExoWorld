using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Core.Menus
{
    public class UIInputProvider : MonoBehaviour, IInputProvider
    {
        [SerializeField] private InputActionAsset inputActions;
        [SerializeField] private string actionMapName = "UI";
        private const string NAVIGATE_ACTION = "Navigate";
        private const string SUBMIT_ACTION = "Submit";
        private const string CANCEL_ACTION = "Cancel";

        private InputActionMap uiActionMap;

        public InputAction NavigationAction { get; private set; }
        public InputAction SubmitAction { get; private set; }
        public InputAction CancelAction { get; private set; }

        private void Awake()
        {
            InitializeActions();
        }

        private void InitializeActions()
        {
            if (inputActions == null)
            {
                Debug.LogError("InputActionAsset not assigned!");
                return;
            }

            uiActionMap = inputActions.FindActionMap(actionMapName);
            
            if (uiActionMap == null)
            {
                Debug.LogError($"Action map '{actionMapName}' not found!");
                return;
            }

            NavigationAction = uiActionMap.FindAction(NAVIGATE_ACTION);
            SubmitAction = uiActionMap.FindAction(SUBMIT_ACTION);
            CancelAction = uiActionMap.FindAction(CANCEL_ACTION);

            ValidateActions();
        }

        private void ValidateActions()
        {
            if (NavigationAction == null) Debug.LogError("Navigate action not found");
            if (SubmitAction == null) Debug.LogError("Submit action not found");
            if (CancelAction == null) Debug.LogError("Cancel action not found");
        }

        private void OnEnable() => uiActionMap?.Enable();
        private void OnDisable() => uiActionMap?.Disable();
    }
}