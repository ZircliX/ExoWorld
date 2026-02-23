using Helteix.ChanneledProperties.Priorities;
using OverBang.ExoWorld.Gameplay.Cameras;
using OverBang.ExoWorld.Gameplay.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Movement
{
    public class PlayerMovement : EntityMovement, IPlayerComponent
    {
        [field: SerializeField] public CameraController CameraController { get; private set; }

        public PlayerController Controller { get; private set; }
        public Animator PlayerAnimator { get; private set; }

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
        
        public void OnSync(PlayerRuntimeContext context)
        {
            Controller = context.playerController;
            PlayerAnimator = context.playerAnimator;
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

        public void ReadInputRun(InputAction.CallbackContext context)
        {
            RunInput = context.performed;
        }

        /*
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
        */

        #endregion
    }
}