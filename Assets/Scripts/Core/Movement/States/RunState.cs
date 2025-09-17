
using UnityEngine;

namespace OverBang.GameName.Movement
{
    [CreateAssetMenu(menuName = "OverBang/Movement/Run")]
    public class RunState : WalkState
    {
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

            return State;
        }

        public override MovementState State => MovementState.Running;
    }
}