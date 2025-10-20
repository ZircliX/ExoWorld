using OverBang.GameName.Gameplay.Cameras;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Movement.States
{
    [CreateAssetMenu(menuName = "OverBang/Movement/Fall")]
    public class FallState : MoveState
    {
        [field: Header("Camera Effects")]
        [field: SerializeField] public FallCameraEffectData CameraEffectData { get; protected set; }
        private float fovModifier;

        public override void Enter(EntityMovement movement)
        {
            fovModifier = 0;
            //Debug.Log($"Entering {State}");

        }

        public override void Exit(EntityMovement movement)
        {
            fovModifier = 0;
        }

        public override MovementState GetNextState(EntityMovement movement)
        {
            if (movement.IsGrounded)
            {
                MovementState nextState = MovementState.Idle;

                if (movement.CrouchInput)
                    nextState = MovementState.Crouching;
                else if (movement.InputDirection.sqrMagnitude > EntityMovement.MIN_THRESHOLD)
                {
                    nextState = MovementState.Walking;
                }

                return nextState;
            }
            if (movement.CanWallRun())
            {
                return MovementState.WallRunning;
            }
            if (movement.CanJump())
            {
                return MovementState.Jumping;
            }
            if (movement.CanDash())
            {
                return MovementState.Dashing;
            }
            if (movement.RunInput)
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
            CameraEffectComposite composite = CameraEffectData.CameraEffectComposite;

            fovModifier = Mathf.Min(
                fovModifier + deltaTime * CameraEffectData.FovMultiplier,
                CameraEffectData.MaxFov - composite.FovScale
            );

            composite.FovScale += fovModifier;
            return composite;
        }

        protected override Vector3 GetGroundNormal(EntityMovement movement)
        {
            return -movement.Gravity.Value.normalized;
        }

        public override MovementState State => MovementState.Falling;
    }
}