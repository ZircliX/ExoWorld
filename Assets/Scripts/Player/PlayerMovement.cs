using DeadLink.Cameras;
using DeadLink.Entities.Movement;
using LTX.ChanneledProperties.Priorities;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RogueLike.Player
{
    public class PlayerMovement : EntityMovement
    {
        #region References

        [field: Header("References")]
        [field: SerializeField] public Camera Camera { get; private set; }
        
        #endregion

        private Coroutine voidEnter;
        

        protected override void Awake()
        {
            base.Awake();
            CameraController.Instance.CameraEffectProperty.AddPriority(stateChannelKey, PriorityTags.High);
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            CameraController.Instance.CameraEffectProperty.Write(stateChannelKey, movementStates[currentStateIndex].GetCameraEffects(this, Time.deltaTime));
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