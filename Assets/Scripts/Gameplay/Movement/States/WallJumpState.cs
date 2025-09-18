using OverBang.GameName.Gameplay.Cameras;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Movement.States
{
    [CreateAssetMenu(menuName = "OverBang/Movement/WallJump")]
    public class WallJumpState : JumpState
    {
        [Header("Wall Jump")]
        [SerializeField] private float diagonalJumpForce;
        [SerializeField] private AnimationCurve diagonalCurve;

        public override Vector3 GetVelocity(EntityMovement movement, float deltaTime, ref float gravityScale)
        {
            float normTime = currentJumpTime / jumpDuration;

            float jumpModifier = jumpCurve.Evaluate(normTime);
            float diagonalJumpModifier = diagonalCurve.Evaluate(normTime);

            Vector3 baseVelocity = base.GetVelocity(movement, deltaTime, ref gravityScale);
            Vector3 gravityNormal = GetGroundNormal(movement);

            if (currentJumpTime <= 0)
            {
                baseVelocity = Vector3.ProjectOnPlane(baseVelocity, gravityNormal);
            }

            currentJumpTime += deltaTime;
            if (currentJumpTime >= jumpDuration)
            {
                movement.SetMovementState(MovementState.Falling);
            }

            Vector3 finalVelocity = gravityNormal * (jumpModifier * jumpForce) +
                                    movement.LastKnownWallNormal * (diagonalJumpModifier * diagonalJumpForce) + baseVelocity;
            //Debug.DrawRay(movement.rb.position, finalVelocity, Color.cyan, 2);

            return finalVelocity;
        }
        
        public override CameraEffectComposite GetCameraEffects(EntityMovement movement, float deltaTime)
        {
            Vector3 cross = Vector3.Cross(movement.WallNormal, movement.Gravity.Value.normalized);
            float dot = Vector3.Dot(cross, movement.CurrentVelocity.Value.normalized);

            CameraEffectComposite comp = CameraEffectData.CameraEffectComposite;
            CameraEffectComposite cameraEffectComposite = new CameraEffectComposite(
                dot > 0 ? comp.Dutch : -comp.Dutch, 
                comp.FovScale, 
                comp.Speed);
            
            return cameraEffectComposite;
        }
        
        public override MovementState State => MovementState.WallJumping;
    }
}