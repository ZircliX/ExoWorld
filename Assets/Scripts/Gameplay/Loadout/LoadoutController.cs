using System;
using System.Collections.Generic;
using KBCore.Refs;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Scene;
using OverBang.ExoWorld.Gameplay.Core.Menus;
using OverBang.ExoWorld.Gameplay.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class LoadoutController : MonoBehaviour, IInputReceiver, IPlayerComponent
    {
        [SerializeField, Self] private WeaponController weaponController;
        [SerializeField, Self] private GadgetController gadgetController;
        
        [SerializeField] private InputActionAsset map;
        [SerializeField] private List<string> cameraInputActionName;
        [SerializeField] private List<string> gameplayInputActionName;
        
        public PlayerController Controller { get; private set; }
        
        private Stack<IInputReceiver> receivers;
        private List<InputAction> actions;
        private List<InputAction> gameplayActions;
        private IInputReceiver current;
        
        private bool activeInputs = true;

        public event Action<bool> OnActiveStateChanged;
        public bool OnlyUI { get; private set; }

        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        private void OnDestroy()
        {
            Controller.LocalGamePlayer.OnStateChanged -= OnStateChange;
        }

        private void OnStateChange(PlayerState state)
        {
            activeInputs = state is not (PlayerState.Down or PlayerState.Dead);
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

            SetOnlyUIState(false);
        }
        
        public void SetOnlyUIState(bool state)
        {
            OnlyUI = state;
            OnActiveStateChanged?.Invoke(state);
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
            if (!activeInputs) return;
            if (OnlyUI) return;
            current.OnLeftInput(context);
        }

        public void OnRightInput(InputAction.CallbackContext context)
        {
            if (!activeInputs) return;
            if (OnlyUI) return;
            current.OnRightInput(context);
        }

        public void OnMiddleDragInput(InputAction.CallbackContext context)
        {
            if (!activeInputs) return;
            if (OnlyUI) return;
            current.OnMiddleDragInput(context);
        }

        public void OnRInput(InputAction.CallbackContext context)
        {
            if (!activeInputs) return;
            if (OnlyUI) return;
            current.OnRInput(context);
        }

        public void OnCInput(InputAction.CallbackContext context)
        {
            if (!activeInputs) return;
            bool scene = SceneLoader.GetCurrentScene().name != GameMetrics.Global.SceneCollection.HubSceneRef.Name;
            if (!scene)
                return;
            
            if (context.performed)
            {
                SwitchReceiver(gadgetController);
                ChangeGameplayInputsState(false);
                SwitchCameraInputs(false);
            }
            current.OnCInput(context);

            if (context.canceled && OnlyUI)
            {
                ChangeGameplayInputsState(true);
                RemoveReceiver(gadgetController);
            }
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
        
        public void OnSync(PlayerRuntimeContext context)
        {
            Controller = context.playerController;
            Controller.LocalGamePlayer.OnStateChanged += OnStateChange;
        }
    }
}