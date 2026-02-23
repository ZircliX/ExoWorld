using System.Collections.Generic;
using KBCore.Refs;
using OverBang.ExoWorld.Gameplay.Player.PlayerHUD;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class LoadoutController : MonoBehaviour, IInputReceiver
    {
        [SerializeField, Self] private WeaponController weaponController;
        [SerializeField, Self] private GadgetController gadgetController;
        
        [SerializeField] private InputActionAsset map;
        [SerializeField] private List<string> cameraInputActionName;
        [SerializeField] private List<string> gameplayInputActionName;
        
        private Stack<IInputReceiver> receivers;
        private List<InputAction> actions;
        private List<InputAction> gameplayActions;
        private IInputReceiver current;
		

        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        private void Awake()
        {
            actions = SearchInputsActionByName(cameraInputActionName);
            gameplayActions = SearchInputsActionByName(gameplayInputActionName);
            
            receivers = new Stack<IInputReceiver>();
            receivers.Push(weaponController);
            current = receivers.Peek();
            weaponController.Initialize(this);
            gadgetController.Initialize(this);
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
                HUD.Instance.ChangeGIStateWheelOpen(false, gameplayActions);
                SwitchCameraInputs(false);
            }
            current.OnCInput(context);
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
        
        public void ChangeGameplayInputsState(bool value)
        {
            HUD.Instance.ChangeGIStateWheelOpen(value, gameplayActions);
        }
        
        private List<InputAction> SearchInputsActionByName(List<string> inputsName)
        {
            using (ListPool<InputAction>.Get(out List<InputAction> inputs))
            {
                foreach (string action in inputsName)
                {
                    InputAction results = map.FindAction(action);
                    if (results != null)
                    {
                        inputs.Add(results);
                    }
                }
                return new List<InputAction>(inputs);
            }
        }

        #endregion
    }
}