using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Core
{
    public class UINavigationManager : MonoBehaviour
    {
        [SerializeField] private EventSystem eventSystem;
        private IInputProvider inputProvider;

        private float navigationCooldown;
        private const float NAVIGATION_COOLDOWN = 0.2f;

        private void Start()
        {
            inputProvider ??= FindFirstObjectByType<UIInputProvider>();

            eventSystem ??= EventSystem.current;

            SubscribeToInputs();
        }

        private void SubscribeToInputs()
        {
            inputProvider.NavigationAction.performed += OnNavigationPerformed;
            inputProvider.SubmitAction.performed += OnSubmitPerformed;
            inputProvider.CancelAction.performed += OnCancelPerformed;
        }

        private void OnDestroy()
        {
            inputProvider.NavigationAction.performed -= OnNavigationPerformed;
            inputProvider.SubmitAction.performed -= OnSubmitPerformed;
            inputProvider.CancelAction.performed -= OnCancelPerformed;
        }

        private void Update()
        {
            navigationCooldown -= Time.deltaTime;
        }

        private void OnNavigationPerformed(InputAction.CallbackContext context)
        {
            if (navigationCooldown > 0 || eventSystem.currentSelectedGameObject == null)
                return;

            Vector2 input = context.ReadValue<Vector2>();
            
            Selectable selected = eventSystem.currentSelectedGameObject.GetComponent<Selectable>();
            if (selected == null) 
                return;

            Selectable next = GetNextSelectable(selected, input);
            if (next == null) 
                return;
            
            eventSystem.SetSelectedGameObject(next.gameObject);
            navigationCooldown = NAVIGATION_COOLDOWN;
        }

        private Selectable GetNextSelectable(Selectable current, Vector2 input)
        {
            return Mathf.Abs(input.x) > Mathf.Abs(input.y)
                ? (input.x > 0.5f ? current.FindSelectableOnRight() : current.FindSelectableOnLeft())
                : (input.y > 0.5f ? current.FindSelectableOnUp() : current.FindSelectableOnDown());
        }

        private void OnSubmitPerformed(InputAction.CallbackContext context)
        {
            if (eventSystem.currentSelectedGameObject?.GetComponent<Button>() is { interactable: true } button)
                button.onClick.Invoke();
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            if (eventSystem.currentSelectedGameObject?.GetComponentInParent<INavigablePanel>() is { } panel)
                panel.InvokeBackClicked();
        }
    }
}