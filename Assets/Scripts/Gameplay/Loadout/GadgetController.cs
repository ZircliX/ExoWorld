using System;
using OverBang.ExoWorld.Gameplay.Abilities;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay
{
    public class GadgetController : NetworkBehaviour, ICaster, IInputReceiver
    {
        public Vector3 Forward => transform.forward;
        private LoadoutController loadoutController;
        private IGadget currentGadget;

        public event Action OnGadgetSelectionBegin; 
        public event Action OnGadgetSelectionEnd; 
        
        
        public void Initialize(LoadoutController loadoutController)
        {
            this.loadoutController = loadoutController;
        }

        public void OnLeftInput(InputAction.CallbackContext context)
        {
            
        }

        public void OnCInput(InputAction.CallbackContext context)
        {
            if (context.performed) StartGadgetSelection();
            if (context.canceled) StopGadgetSelection();
        }

        
        private void StartGadgetSelection()
        {
            OnGadgetSelectionBegin?.Invoke();
        }

        private void StopGadgetSelection()
        {
            OnGadgetSelectionEnd?.Invoke();
            OnEnd();
        }
        
        private void OnEnd()
        {
            loadoutController.RemoveReceiver(this);
        }

    }
}