using DeadLink.Cameras;
using DeadLink.Cameras.Data;
using DeadLink.Entities.Movement;
using UnityEngine;

namespace RogueLike.Player.States
{
    [CreateAssetMenu(menuName = "RogueLike/Movement/Jump")]
    public class JumpState : FallState
    {
        [Header("Jump")]
        [SerializeField] protected float jumpForce;
        [SerializeField] protected float jumpDuration;
        [SerializeField] protected AnimationCurve jumpCurve;
        
        protected float currentJumpTime;
        
        public override void Enter(EntityMovement movement)
        {
            movement.UseJump();
            base.Enter(movement);
            //Debug.Log($"Entering {State}");
            
            currentJumpTime = 0;
        }

        public override void Exit(EntityMovement movement)
        {
            base.Exit(movement);
            currentJumpTime = 0;
        }

        public override Vector3 GetVelocity(EntityMovement movement, float deltaTime, ref float gravityScale)
        {
            float normTime = currentJumpTime / jumpDuration;

            float jumpModifier = jumpCurve.Evaluate(normTime);

            Vector3 baseVelocity = base.GetVelocity(movement, deltaTime, ref gravityScale);
            Vector3 gravityNormal = GetGroundNormal(movement);

            if (currentJumpTime <= 0)
            {
                //Debug.Log($"before : {baseVelocity} ");
                baseVelocity = Vector3.ProjectOnPlane(baseVelocity, gravityNormal);
                //Debug.Log($"after : {baseVelocity} ");
            }

            currentJumpTime += deltaTime;
            if (currentJumpTime >= jumpDuration)
                movement.SetMovementState(MovementState.Falling);

            Vector3 finalVelocity = gravityNormal * (jumpModifier * jumpForce) + baseVelocity;
            //Debug.Log(finalVelocity);
            Debug.DrawRay(movement.Position, finalVelocity * 2, Color.red);
            
            return finalVelocity;
        }

        public override MovementState GetNextState(EntityMovement movement)
        {
            float dot = Vector3.Dot(movement.CurrentVelocity.Value.normalized, movement.Gravity.Value.normalized);

            if (dot > 0f)
            {
                return MovementState.Falling;
            }
            if (movement.CanWallRun())
            {
                return MovementState.WallRunning;
            }
            if (movement.CanDash())
            {
                return MovementState.Dashing;
            }

            return State;
        }
        
        public override CameraEffectComposite GetCameraEffects(EntityMovement movement, float deltaTime)
        {
            return CameraEffectData.CameraEffectComposite;
        }

        public override MovementState State => MovementState.Jumping;
    }
}