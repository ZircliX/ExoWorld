using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetUiSelector : MonoBehaviour
    { 
        [Header("Menu Settings")]
        [SerializeField, Space] private float radius = 150f;
        
        [Header("Selection Settings")]
        [SerializeField, Space] private float selectionThreshold = 2f;
        [SerializeField] private float deltaSensitivity = 1f; 

        private List<GadgetUi> GadgetUis => controllerUI.gadgetUis;
        private int itemCount => GadgetUis.Count;
        private int currentSelection;
        private Vector2 centerPosition;
        private Vector2 accumulatedDelta; 
        
        private GadgetControllerUI controllerUI;
        private GadgetUi currentSelectedGadget;

        public void Initialize(GadgetControllerUI controllerUI)
        {
            this.controllerUI = controllerUI;
        }
        
        public void StartSelection()
        {
            accumulatedDelta = Vector2.zero; 
            currentSelection = 1;
        }
        
        private void Update()
        {
            // Accumulate delta movement
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            accumulatedDelta += mouseDelta * deltaSensitivity;
                
            UpdateSelection();
        }
        
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
            int selectedIndex = Mathf.RoundToInt(angle / anglePerItem) % itemCount;
            SelectItem(selectedIndex);
        }
        
        
        private void SelectItem(int selectedIndex)
        {
            if (selectedIndex >= 0 && selectedIndex < GadgetUis.Count)
            {
                Debug.Log($"Selected: Item {selectedIndex}");
                OnItemSelected(selectedIndex);
            }
        }
        
        private void OnItemSelected(int index)
        {
            if (currentSelectedGadget == GadgetUis[index]) return;
            
            currentSelectedGadget.DeselectThisGadget();
            currentSelectedGadget = GadgetUis[index];
            controllerUI.SetCurrentSelectedGadget(currentSelectedGadget.data);
            
            DOVirtual.DelayedCall(0.15f, () =>
            {
                currentSelectedGadget = GadgetUis[index];
                currentSelectedGadget.SelectThisGadget();
            });
        }
    }
}