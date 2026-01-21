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
        
        
        
        public void Initialize(LoadoutController loadoutController)
        {
            this.loadoutController = loadoutController;
        }

        public void OnLeftInput(InputAction.CallbackContext context)
        {
            
        }

        public void OnCInput(InputAction.CallbackContext context)
        {
            
        }

        public void OnEnd()
        {
            
        }

    }
}