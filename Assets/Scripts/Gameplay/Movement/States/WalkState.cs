using OverBang.ExoWorld.Gameplay.Cameras.Composits;
using OverBang.ExoWorld.Gameplay.Cameras.Data;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Movement
{
    [CreateAssetMenu(menuName = "OverBang/Movement/Walk")]
    public class WalkState : MoveState
    {
        [field: Header("Camera Effects")]
        [field: SerializeField] public CameraEffectData CameraEffectData { get; protected set; }
        [field: SerializeField] public CameraShakeData CameraShakeData { get; protected set; }
        
        public override Vector3 GetVelocity(EntityMovement movement, float deltaTime, ref float gravityScale)
        {
            Vector3 velocity = base.GetVelocity(movement, deltaTime, ref gravityScale);

            //const float snapForce = 2;
            //velocity += movement.Gravity.Value.normalized * snapForce * deltaTime;
            
            return velocity;
        }
        
        public override void Enter(EntityMovement movement)
        {
            movement.PlayerHeight.Write(this, (movement.BaseCapsuleHeight, movement.BaseHeadHeight));
            
            if (movement is PlayerMovement playerMovement && playerMovement.PlayerAnimator != null)
            {
                playerMovement.PlayerAnimator.SetBool("Run", true);
            }
        }

        public override void Exit(EntityMovement movement)
        {
            base.Exit(movement);
            if (movement is PlayerMovement playerMovement && playerMovement.PlayerAnimator != null)
            {
                playerMovement.PlayerAnimator.SetBool("Run", false);
            }
        }

        public override MovementState GetNextState(EntityMovement movement)
        {
            if (!movement.IsGrounded)
            {
                return MovementState.Falling;
            }
            if (movement.CanJump())
            {
                return MovementState.Jumping;
            }
            if (movement.CrouchInput)
            {
                return MovementState.Crouching;
            }
            if (movement.CanDash())
            {
                return MovementState.Dashing;
            }
            if (movement.InputDirection.sqrMagnitude < EntityMovement.MIN_THRESHOLD)
            {
                //Debug.Log("HAAAAAAA");
                return MovementState.Idle;
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
            return CameraEffectData.CameraEffectComposite;
        }

        public override CameraShakeComposite GetCameraShakes(EntityMovement movement, float deltaTime)
        {
            return CameraShakeData.CameraShakeComposite;
        }

        public override MovementState State => MovementState.Walking;
    }
}