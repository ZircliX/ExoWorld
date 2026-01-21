using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay
{
    public class LoadoutController : MonoBehaviour, IInputReceiver
    {
        [SerializeField] private WeaponController weaponController;
        [SerializeField] private GadgetController gadgetController;

        private IInputReceiver current;


        private void Awake()
        {
            weaponController.Initialize(this);
            gadgetController.Initialize(this);
        }

        public void SwitchReceiver(IInputReceiver receiver)
        {
            current = receiver; 
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
            SwitchReceiver(gadgetController);
            current.OnCInput(context);
        }

        #endregion
    }
}