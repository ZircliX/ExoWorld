using LTX.ChanneledProperties.Priorities;
using OverBang.GameName.Cameras;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Movement
{
    public class PlayerMovement : EntityMovement
    {
        #region References

        [field: Header("References")]
        [field: SerializeField] public Camera Camera { get; private set; }
        [field: SerializeField] public CameraController CameraController { get; private set; }
        
        #endregion

        protected override void Awake()
        {
            base.Awake();
            CameraController.CameraEffectProperty?.AddPriority(stateChannelKey, PriorityTags.High);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            CameraController.CameraEffectProperty.Write(stateChannelKey, movementStates[currentStateIndex].GetCameraEffects(this, Time.deltaTime));
        }
        
        #region Inputs

        public void ReadInputMove(InputAction.CallbackContext context)
        {
            Vector2 input = context.ReadValue<Vector2>();
            InputDirection = new Vector3(input.x, 0, input.y);
        }

        public void ReadInputJump(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                jumpInputPressed = true;
            }
            else if (context.canceled)
            {
                jumpInputPressed = false;
            }
        }

        public void ReadInputCrouch(InputAction.CallbackContext context)
        {
            CrouchInput = context.performed;
        }

        public void ReadInputSlide(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                slideInput = coyoteTime;
            }
            else if (context.canceled)
            {
                slideInput = 0;
            }
        }

        public void ReadInputDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                DashInput = true;
            }
            else if (context.canceled)
            {
                DashInput = false;
            }
        }

        #endregion

    }
}