using DeadLink.Cameras;
using DeadLink.Cameras.Data;
using DeadLink.Entities.Movement;
using UnityEngine;

namespace RogueLike.Player.States
{
    [CreateAssetMenu(menuName = "RogueLike/Movement/Walk")]
    public class WalkState : MoveState
    {
        [field: Header("Camera Effects")]
        [field: SerializeField] public CameraEffectData CameraEffectData { get; protected set; }
        
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

        public override MovementState State => MovementState.Walking;
    }
}