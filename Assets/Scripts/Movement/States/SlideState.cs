using OverBang.GameName.Cameras;
using UnityEngine;

namespace OverBang.GameName.Movement
{
    [CreateAssetMenu(menuName = "RogueLike/Movement/Slide")]
    public class SlideState : MovementStateBehavior
    {
        [field: Header("Camera Effects")]
        [field: SerializeField] public CameraEffectData CameraEffectData { get; protected set; }

        [Header("Speed")]
        [SerializeField] private float slideSpeed;
        [SerializeField] private float slideMinSpeed;
        [SerializeField] private AnimationCurve accelerationCurve;
        [SerializeField] private float accelerationDuration;
        [SerializeField] private float decelerationStrength;
        [SerializeField] private float decelerationThreshold;

        [Header("Slopes")]
        [SerializeField] private AnimationCurve slopeCurve;
        [SerializeField] private float slopeModifier;
        [SerializeField, Range(0, 1)] private float minSlopeAngle;

        [Header("Sliding")]
        [SerializeField] private float maxSlideTime = 1.5f;

        [Header("Height")]
        [SerializeField] private float crouchCapsuleHeight = 1f;
        [SerializeField] private  float crouchHeadHeight = 0f;

        private float currentSlideTime;
        private Vector3 direction;

        public override void Dispose(EntityMovement movement)
        {

        }

        public override void Enter(EntityMovement movement)
        {
            currentSlideTime = 0;
            direction = GetWorldInputs(movement);
        }

        public override void Exit(EntityMovement movement)
        {
            currentSlideTime = 0;
        }

        public override Vector3 GetVelocity(EntityMovement movement, float deltaTime, ref float gravityScale)
        {
            Vector3 velocity = Vector3.zero;
            Vector3 projectionPlaneNormal = GetGroundNormal(movement);

            //Acceleration
            if (currentSlideTime < accelerationDuration)
            {
                Vector3 projectOnPlane = Vector3.ProjectOnPlane(direction, projectionPlaneNormal);
                Vector3 newVelocity = projectOnPlane * (accelerationCurve.Evaluate(currentSlideTime / accelerationDuration) * slideSpeed);
                velocity = newVelocity;
                Debug.DrawRay(movement.Position, velocity, Color.magenta);
            }
            //Deceleration
            else
            {
                Vector3 currentVelocity = movement.StateVelocity;
                float dotProduct = Vector3.Dot(movement.Gravity.Value.normalized, Vector3.ProjectOnPlane(currentVelocity, projectionPlaneNormal).normalized);

                if (Mathf.Abs(dotProduct) <= minSlopeAngle)
                    velocity = Vector3.MoveTowards(currentVelocity, direction * slideMinSpeed, decelerationStrength * deltaTime);
                else
                    velocity = currentVelocity;

                float modifier = slopeCurve.Evaluate(dotProduct) * slopeModifier * deltaTime;
                velocity += direction.normalized * modifier;
            }

            currentSlideTime += deltaTime;
            return velocity;
        }

        public override MovementState GetNextState(EntityMovement movement)
        {
            if (!movement.IsGrounded)
            {
                //Debug.Break();
                return MovementState.Falling;
            }
            if (movement.CanJump() && !movement.IsTouchingCeiling)
            {
                return MovementState.Jumping;
            }

            Vector3 projectionPlaneNormal = GetGroundNormal(movement);
            Vector3 projectOnPlane = Vector3.ProjectOnPlane(movement.StateVelocity, projectionPlaneNormal);

            if (projectOnPlane.sqrMagnitude < slideMinSpeed * slideMinSpeed + decelerationThreshold)
            {
                if (!movement.IsTouchingCeiling)
                    return MovementState.Running;
                else
                    return MovementState.Crouching;
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

        public override MovementState State => MovementState.Sliding;
    }
}