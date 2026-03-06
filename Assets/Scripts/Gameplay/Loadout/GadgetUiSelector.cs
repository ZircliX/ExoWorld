using System.Collections.Generic;
using TMPro;
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
        private int ItemCount => GadgetUis.Count;
        private int selectedIndex;
        private Vector2 centerPosition;
        private Vector2 accumulatedDelta; 
        
        private GadgetControllerUI controllerUI;
        private GadgetUi currentSelectedGadget;
        
        public void Initialize(GadgetControllerUI controllerUI)
        {
            this.controllerUI = controllerUI;
            controllerUI.OnGadgetUiSelectionEnd += OnItemSelected;
        }
        
        private void OnDisable()
        {
            controllerUI.OnGadgetUiSelectionEnd -= OnItemSelected;
        }
        
        
        public void StartSelection()
        {
            if (currentSelectedGadget !=null && !currentSelectedGadget.CheckSelectiveness())
            {
                Debug.Log(currentSelectedGadget.CheckSelectiveness());
                currentSelectedGadget.DeselectThisGadget();
                currentSelectedGadget.Mask();
                currentSelectedGadget = null;
            }
            accumulatedDelta = Vector2.zero;
        }
        
        private void Update()
        {
            if (!controllerUI.Controller.IsSelecting) return;
            
            Vector2 mouseDelta = Pointer.current.delta.ReadValue();
            
            if (mouseDelta.sqrMagnitude > selectionThreshold)
            {
                accumulatedDelta = mouseDelta * deltaSensitivity;
                UpdateSelection();
            }
        }

        private void UpdateSelection()
        {
            if (accumulatedDelta.magnitude < selectionThreshold)
            {
                //Debug.Log("no movement, no new selection");
                return;
            }
            
            float angle = Mathf.Atan2(accumulatedDelta.x, accumulatedDelta.y) * Mathf.Rad2Deg;
            if (angle < 0) angle += 360f;
            
            float anglePerItem = 360f / ItemCount;
            selectedIndex = Mathf.RoundToInt(angle / anglePerItem) % ItemCount;
            
            float targetAngle = selectedIndex * anglePerItem;
            
            GadgetUi supposedGadget = GadgetUis[selectedIndex];
            if (supposedGadget.CheckSelectiveness())
            {
                wheelPointerImage.transform.rotation = Quaternion.Euler(0f, 0f, -targetAngle);
            }
            
            SelectItemUI();
        }
        
        
        private void SelectItemUI()
        {
            if (selectedIndex >= 0 && selectedIndex < GadgetUis.Count)
            {
                UpdateSelectedGadget(selectedIndex);
            }
        }

        private void UpdateSelectedGadget(int index)
        {
            GadgetUi newGadget = GadgetUis[index];
            if (newGadget == currentSelectedGadget || !GadgetUis[index].isSelectable) 
                return;
            
            if (currentSelectedGadget != null)
                currentSelectedGadget.DeselectThisGadget();
            
            currentSelectedGadget = newGadget;
            currentSelectedGadget.SelectThisGadget();
        }
        
        private void OnItemSelected()
        {
            if (currentSelectedGadget == null)
            {
                controllerUI.SetCurrentSelectedGadget(null);
                return;
            }
                
            controllerUI.SetCurrentSelectedGadget(currentSelectedGadget.data);
        }
    }
}