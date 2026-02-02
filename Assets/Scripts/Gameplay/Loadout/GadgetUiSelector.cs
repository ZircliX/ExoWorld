using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetUiSelector : MonoBehaviour
    { 
        [Header("Menu Settings")]
        [SerializeField, Space] private float radius = 150f;
        
        [Header("Selection Settings")]
        [SerializeField, Space] private float selectionThreshold = 0.5f;
        [SerializeField] private float deltaSensitivity = 1f; 
        [SerializeField] private Image wheelPointerImage; 

        private List<GadgetUi> GadgetUis => controllerUI.GadgetUis;
        private int itemCount => GadgetUis.Count;
        private int currentSelection;
        private Vector2 centerPosition;
        private Vector2 accumulatedDelta; 
        
        private GadgetControllerUI controllerUI;
        private GadgetUi currentSelectedGadget;
        
        public void Initialize(GadgetControllerUI controllerUI)
        {
            this.controllerUI = controllerUI;
            controllerUI.OnGadgetUiSelectionEnd += SelectItem;
        }
        
        private void OnDisable()
        {
            controllerUI.OnGadgetUiSelectionEnd -= SelectItem;
        }
        
        
        public void StartSelection()
        {
            accumulatedDelta = Vector2.zero;
            currentSelection = 1;
        }
        
        private void Update()
        {
            // Accumulate delta movement
            Vector2 mouseDelta = Pointer.current.delta.ReadValue();
            
            if (mouseDelta.sqrMagnitude > selectionThreshold)
            {
                accumulatedDelta = mouseDelta * deltaSensitivity;
                UpdateSelection();
            }
                
        }

        private int selectedIndex;
        private void UpdateSelection()
        {
            // Check if mouse moved far enough from center
            if (accumulatedDelta.magnitude < selectionThreshold)
            {
                return;
            }
            
            // Calculate angle from accumulated delta
            float angle = Mathf.Atan2(accumulatedDelta.x, accumulatedDelta.y) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;
            
            // Find closest menu item
            float anglePerItem = 360f / itemCount;
            selectedIndex = Mathf.RoundToInt(angle / anglePerItem) % itemCount;
            
            float targetAngle = selectedIndex * anglePerItem;
            wheelPointerImage.transform.rotation = Quaternion.Euler(0f, 0f, -targetAngle);
            
            SelectItem();
        }
        
        
        private void SelectItem()
        {
            if (selectedIndex >= 0 && selectedIndex < GadgetUis.Count)
            {
                OnItemSelected(selectedIndex);
            }
        }
        
        private void OnItemSelected(int index)
        {
            GadgetUi maybeNewGadget = GadgetUis[index];
            if (maybeNewGadget == currentSelectedGadget || !GadgetUis[index].isSelectable) return;
            
            if (currentSelectedGadget != null)
                currentSelectedGadget.DeselectThisGadget();
            
            currentSelectedGadget = maybeNewGadget;
            
            controllerUI.SetCurrentSelectedGadget(currentSelectedGadget.data);
            currentSelectedGadget.SelectThisGadget();

        }
    }
}