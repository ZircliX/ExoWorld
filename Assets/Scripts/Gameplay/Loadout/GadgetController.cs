using System;
using OverBang.ExoWorld.Core;
using OverBang.ExoWorld.Gameplay.Abilities;
using OverBang.ExoWorld.Gameplay.PlayerHUD;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay
{
    public class GadgetController : NetworkBehaviour, ICaster, IInputReceiver
    {
        [SerializeField] private GadgetControllerUI gadgetControllerUI;
        public Vector3 Forward => transform.forward;
        private LoadoutController loadoutController;
        
        private IGadget currentGadget;

        public event Action OnGadgetSelectionBegin; 
        public event Action OnGadgetSelectionEnd; 
        
        
        public void Initialize(LoadoutController loadoutController)
        {
            this.loadoutController = loadoutController;
        }


        private void StartGadgetSelection()
        {
            HUD.Instance.SetCursorState(true);
            OnGadgetSelectionBegin?.Invoke();
        }

        public void SelectCurrentGadget(IGadget gadget)
        {
            currentGadget = gadget;
        }
        
        private void StopGadgetSelection()
        {
            HUD.Instance.SetCursorState(false);
            OnGadgetSelectionEnd?.Invoke();
            
            //currentGadget.Initialize();
            //currentGadget.Begin();
        }
        
        private void OnEnd()
        {
            loadoutController.RemoveReceiver(this);
        }

        #region Inputs
        
            public void OnLeftInput(InputAction.CallbackContext context)
            {
                
            }

            public void OnCInput(InputAction.CallbackContext context)
            {
                if (context.performed) StartGadgetSelection();
                if (context.canceled) StopGadgetSelection();
            }
            
        #endregion
    }
}