using System;
using System.Collections.Generic;
using KBCore.Refs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay
{
    public class LoadoutController : MonoBehaviour, IInputReceiver
    {
        [SerializeField, Self] private WeaponController weaponController;
        [SerializeField, Self] private GadgetController gadgetController;

        private Stack<IInputReceiver> receivers;
        private IInputReceiver current;
		

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void Awake()
        {
            receivers = new Stack<IInputReceiver>();
            receivers.Push(weaponController);
            current = receivers.Peek();
            weaponController.Initialize(this);
            gadgetController.Initialize(this);
        }

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
            }
        }
        
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
            if (context.performed) SwitchReceiver(gadgetController);
            
            current.OnCInput(context);
        }

        #endregion
    }
}