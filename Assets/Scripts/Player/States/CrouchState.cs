using DeadLink.Cameras;
using DeadLink.Cameras.Data;
using DeadLink.Entities.Movement;
using UnityEngine;

namespace RogueLike.Player.States
{
    [CreateAssetMenu(menuName = "RogueLike/Movement/Crouch")]
    public class CrouchState : MoveState
    {
        [field: Header("Camera Effects")]
        [field: SerializeField] public CameraEffectData CameraEffectData { get; protected set; }

        [Header("Height")]
        [SerializeField] private float crouchCapsuleHeight = 0.5f;
        [SerializeField] private  float crouchHeadHeight = 0f;

        public override void Enter(EntityMovement movement)
        {
            base.Enter(movement);
        }

        public override void Exit(EntityMovement movement)
        {
            base.Exit(movement);
        }

        public override MovementState GetNextState(EntityMovement movement)
        {
            if (!movement.IsGrounded)
            {
                return MovementState.Falling;
            }
            if (!movement.CrouchInput && !movement.IsTouchingCeiling)
            {
                return MovementState.Idle;
            }

            return State;
        }

        public override (float, float) GetHeight(EntityMovement movement)
        {
            return (crouchCapsuleHeight, crouchHeadHeight);
        }

        public override CameraEffectComposite GetCameraEffects(EntityMovement movement, float deltaTime)
        {
            return CameraEffectData.CameraEffectComposite;
        }

        public override MovementState State => MovementState.Crouching;
    }
}