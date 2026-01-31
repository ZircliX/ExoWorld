using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.LowLevel;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class LoadoutController : MonoBehaviour, IInputReceiver
    {
        [SerializeField, Self] private WeaponController weaponController;
        [SerializeField, Self] private GadgetController gadgetController;
        
        [SerializeField] private InputActionAsset map;
        [SerializeField] private List<string> cameraInputActionName;
        
        private Stack<IInputReceiver> receivers;
        private List<InputAction> actions;
        private IInputReceiver current;
		

        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        private void Awake()
        {
            actions = new List<InputAction>();

            foreach (string action in cameraInputActionName)
            {
                InputAction Results = map.FindAction(action);
                if (Results != null)
                {
                    actions.Add(Results);
                }
            }
            
            receivers = new Stack<IInputReceiver>();
            receivers.Push(weaponController);
            current = receivers.Peek();
            weaponController.Initialize(this);
            gadgetController.Initialize(this);
        }

        public void SwitchCameraInputs(bool value)
        {
            foreach (InputAction action in actions)
            {
                if (value)
                {
                    action.Enable();
                }
                else
                {
                    action.Disable();
                }
            }
        }

        #region Receivers

            public void SwitchReceiver(IInputReceiver receiver)
            {
                receivers.Push(receiver);
                current = receiver; 
            }

            public void RemoveReceiver(IInputReceiver receiver)
            {
                if (receivers.Peek() == receiver)
                {
                    receivers.Pop();
                    current = receivers.Peek();

                    SwitchCameraInputs(true);
                }
            }

        #endregion
        
        #region Inputs
        public void OnLeftInput(InputAction.CallbackContext context)
        {
            current.OnLeftInput(context);
        }

        public void OnRightInput(InputAction.CallbackContext context)
        {
            current.OnRightInput(context);
        }

        public void OnMiddleDragInput(InputAction.CallbackContext context)
        {
            current.OnMiddleDragInput(context);
        }

        public void OnRInput(InputAction.CallbackContext context)
        {
            current.OnRInput(context);
        }

        public void OnCInput(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                SwitchReceiver(gadgetController);
                SwitchCameraInputs(false);
            }
            current.OnCInput(context);
        }

        #endregion
    }
}