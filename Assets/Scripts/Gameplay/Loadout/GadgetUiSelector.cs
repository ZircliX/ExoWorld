using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class GadgetUiSelector : MonoBehaviour
    { 
        [SerializeField] private TMP_Text currentSelectedGadgetText;
        [SerializeField] private GadgetControllerUI controllerUI;
        
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
        
        public GadgetUi CurrentSelectedGadget { get; private set; }

        private void OnEnable()
        {
            controllerUI.OnGadgetUiSelectionBegin += StartSelection;
            controllerUI.OnGadgetUiSelectionEnd += OnItemSelected;
            controllerUI.Controller.OnGadgetCasted += UpdateCurrentGadgetAmount;
        }
        
        private void OnDisable()
        {
            controllerUI.OnGadgetUiSelectionBegin += StartSelection;
            controllerUI.OnGadgetUiSelectionEnd -= OnItemSelected;
            controllerUI.Controller.OnGadgetCasted -= UpdateCurrentGadgetAmount;
        }
        
        private void StartSelection()
        {
            if (CurrentSelectedGadget !=null && !CurrentSelectedGadget.CheckSelectiveness())
            {
                Debug.Log(CurrentSelectedGadget.CheckSelectiveness());
                CurrentSelectedGadget.DeselectThisGadget();
                CurrentSelectedGadget.Mask();
                CurrentSelectedGadget = null;
            }
            accumulatedDelta = Vector2.zero;
        }
        
        private void Update()
        {
            if (controllerUI == null || controllerUI.Controller == null)
            {
                Debug.LogError("GadgetUiSelector is missing a reference to a GadgetControllerUI!");
                return;
            }

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
            if (newGadget == CurrentSelectedGadget || !GadgetUis[index].isSelectable) 
                return;
            
            if (CurrentSelectedGadget != null)
                CurrentSelectedGadget.DeselectThisGadget();
            
            CurrentSelectedGadget = newGadget;
            UpdateCurrentGadgetAmount();
            
            CurrentSelectedGadget.SelectThisGadget();
        }
        
        private void OnItemSelected()
        {
            if (!controllerUI.Controller.IsSelecting)
                return;
            
            if (CurrentSelectedGadget == null || CurrentSelectedGadget.data == null)
            {
                controllerUI.SetCurrentSelectedGadget(null);
                return;
            }
                
            controllerUI.SetCurrentSelectedGadget(CurrentSelectedGadget.data);
        }

        private void UpdateCurrentGadgetAmount()
        {
            int currentAmount = controllerUI.Controller.Player.GadgetInventory.GetGadgetCount(CurrentSelectedGadget.data, out int amount) ? amount : 0;
            currentSelectedGadgetText.text = currentAmount.ToString();
        }
    }
}