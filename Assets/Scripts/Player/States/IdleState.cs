using DeadLink.Cameras;
using DeadLink.Cameras.Data;
using DeadLink.Entities.Movement;
using UnityEngine;

namespace RogueLike.Player.States
{
    [CreateAssetMenu(menuName = "RogueLike/Movement/Idle")]
    public class IdleState : MovementStateBehavior
    {
        [field: Header("Camera Effects")]
        [field: SerializeField] public CameraEffectData CameraEffectData { get; protected set; }

        public override void Dispose(EntityMovement movement)
        {
        }

        public override void Enter(EntityMovement movement)
        {
            movement.PlayerHeight.Write(this, (movement.BaseCapsuleHeight, movement.BaseHeadHeight));
        }

        public override void Exit(EntityMovement movement)
        {
        }

        public override Vector3 GetVelocity(EntityMovement movement, float deltaTime, ref float gravityScale)
        {
            gravityScale = 0;
            return Vector3.zero;
        }

        public override MovementState GetNextState(EntityMovement movement)
        {
            if (!movement.IsGrounded)
            {
                return MovementState.Falling;
            }
            if (movement.CanDash())
            {
                return MovementState.Dashing;
            }
            if (movement.CanJump())
            {
                return MovementState.Jumping;
            }
            if (movement.CrouchInput)
            {
                return MovementState.Crouching;
            }
            if (movement.InputDirection.sqrMagnitude > PlayerMovement.MIN_THRESHOLD)
            {
                return MovementState.Running;
            }

            return State;
        }

        public override (float, float) GetHeight(EntityMovement movement)
        {
            return (movement.BaseCapsuleHeight, movement.BaseHeadHeight);
        }

        public override CameraEffectComposite GetCameraEffects(EntityMovement movement, float deltaTime)
        {
            return CameraEffectData.CameraEffectComposite;
        }

        public override MovementState State => MovementState.Idle;
    }
}