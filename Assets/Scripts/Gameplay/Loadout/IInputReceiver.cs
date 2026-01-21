using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay
{
    public interface IInputReceiver
    {
        public virtual void OnLeftInput(InputAction.CallbackContext context){}
        public virtual void OnRightInput(InputAction.CallbackContext context){}
        public virtual void OnMiddleDragInput(InputAction.CallbackContext context){}
        public virtual void OnRInput(InputAction.CallbackContext context){}
        public virtual void OnCInput(InputAction.CallbackContext context){}

    }
}