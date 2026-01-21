
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay
{
    [CreateAssetMenu(menuName = "OverBang/Movement/Run")]
    public class RunState : WalkState
    {
        public override void Enter(EntityMovement movement)
        {
            base.Enter(movement);
            
            if (movement is PlayerMovement playerMovement)
            {
                playerMovement.PlayerAnimator.SetBool("Run", true);
            }
        }

        public override void Exit(EntityMovement movement)
        {
            base.Exit(movement);
            if (movement is PlayerMovement playerMovement)
            {
                playerMovement.PlayerAnimator.SetBool("Run", false);
            }
        }

        public override MovementState GetNextState(EntityMovement movement)
        {
            //Debug.Log(movement.WantsToSlide);
            //Debug.Log($"grounded = {movement.IsGrounded}");
            
            if (!movement.IsGrounded)
            {
                return MovementState.Falling;
            }
            if (movement.CrouchInput)
            {
                return MovementState.Crouching;
            }
            if (movement.CanJump())
            {
                return MovementState.Jumping;
            }
            if (movement.CanSlide())
            {
                return MovementState.Sliding;
            }
            if (movement.CanDash())
            {
                return MovementState.Dashing;
            }
            if (movement.InputDirection.sqrMagnitude < EntityMovement.MIN_THRESHOLD)
            {
                return MovementState.Idle;
            }
            if (!movement.RunInput)
            {
                return MovementState.Walking;
            }

            return State;
        }

        public override MovementState State => MovementState.Running;
    }
}