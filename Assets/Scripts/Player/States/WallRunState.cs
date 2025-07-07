using DeadLink.Cameras;
using DeadLink.Cameras.Data;
using DeadLink.Entities.Movement;
using Unity.Cinemachine;
using UnityEngine;

namespace RogueLike.Player.States
{
    [CreateAssetMenu(menuName = "RogueLike/Movement/WallRun")]
    public class WallRunState : MovementStateBehavior
    {
        [field: Header("Camera Effects")]
        [field: SerializeField] public CameraEffectData CameraEffectData { get; protected set; }

        [Header("Speed")]
        [SerializeField] private float wallrunSpeed;
        [SerializeField] private float minWallrunSpeed;
        [SerializeField] private float directionControl;

        [Header("Acceleration")]
        [SerializeField] private AnimationCurve accelerationCurve;
        [SerializeField] private float accelerationDuration;
        [SerializeField] private float acceleration;
        private float currentAcceleration;

        [Header("Deceleration")]
        [SerializeField] private AnimationCurve decelerationCurve;
        [SerializeField] private float decelerationDuration;
        [SerializeField] private float deceleration;
        [SerializeField] private float decelerationThreshold;
        private float currentDeceleration;

        [Header("Wall")]
        [SerializeField] private float wallPull;
        [SerializeField] private float wallSnapDistance;

        private Vector3 direction;

        public override void Dispose(EntityMovement movement)
        {
        }

        public override void Enter(EntityMovement movement)
        {
            direction = movement.StateVelocity.sqrMagnitude > 0.1f ? movement.StateVelocity.normalized : Vector3.zero;
            
            currentAcceleration = 0;
            currentDeceleration = 0;
        }

        public override void Exit(EntityMovement movement)
        {
            currentAcceleration = 0;
            currentDeceleration = 0;
        }

        public override CameraEffectComposite GetCameraEffects(EntityMovement movement, float deltaTime)
        {
            Vector3 cross = Vector3.Cross(movement.WallNormal, movement.Gravity.Value.normalized);

            float dot = Vector3.Dot(cross, movement.CameraTransform.forward);

            CameraEffectComposite comp = CameraEffectData.CameraEffectComposite;
            CameraEffectComposite cameraEffectComposite = new CameraEffectComposite(
                dot > 0 ? comp.Dutch : -comp.Dutch,
                comp.FovScale,
                comp.Speed);

            return cameraEffectComposite;
        }

        public override Vector3 GetVelocity(EntityMovement movement, float deltaTime, ref float gravityScale)
        {
            gravityScale = 0;
            direction = movement.StateVelocity.sqrMagnitude > 0.1f ? movement.StateVelocity.normalized : Vector3.zero;
            Vector3 lastVelocity = movement.StateVelocity;

            //Calculate wallrun direction
            Vector3 alongWallDirection = Vector3.Cross(movement.WallNormal, movement.Gravity).normalized;

            float angle = Vector3.Dot(alongWallDirection, lastVelocity);
            if (angle < 0)
            {
                //invert direction
                alongWallDirection = -alongWallDirection;
            }

            Debug.DrawRay(movement.rb.position, alongWallDirection * 10, Color.green);
            //Debug.Break();

            //Snap to Wall
            float distFromWall = (movement.Position - movement.WallContactPoint).sqrMagnitude - movement.CapsuleCollider.radius;
            if (distFromWall > wallSnapDistance)
            {
                Vector3 snapDeltaMovement = -movement.WallNormal * (wallPull * deltaTime);
                movement.rb.position += snapDeltaMovement;
            }

            //Velocities calculation
            Vector3 wallPullForce = wallPull * -movement.WallNormal;
            Vector3 wallVelocity = lastVelocity.ProjectOntoPlane(movement.WallNormal);
            Vector3 targetSpeed = alongWallDirection * wallrunSpeed;

            //Sqr Magnitudes
            float wallVelocitySqrMagnitude = wallVelocity.sqrMagnitude;
            float directionSqrMagnitude = direction.sqrMagnitude;

            //Wanted speed
            if (Mathf.Approximately(directionSqrMagnitude, wallVelocitySqrMagnitude))
            {
                return targetSpeed;
            }
            //Accelerate
            float modifier;
            if (wallVelocitySqrMagnitude < directionSqrMagnitude)
            {
                currentAcceleration += deltaTime;
                currentDeceleration = 0;
                modifier = accelerationCurve.Evaluate(currentAcceleration / accelerationDuration) * acceleration;
            }
            //Decelerate
            else
            {
                currentDeceleration += deltaTime;
                currentAcceleration = 0;
                modifier = decelerationCurve.Evaluate(currentDeceleration / decelerationDuration) * deceleration;
            }

            Vector3 finalVelocity = Vector3.Lerp(wallVelocity, targetSpeed, modifier * deltaTime);
            if (finalVelocity.y <= 0)
            {
                finalVelocity = finalVelocity.ProjectOntoPlane(movement.Gravity.Value.normalized);
            }

            return finalVelocity;
        }

        public override MovementState GetNextState(EntityMovement movement)
        {
            if (movement.CanJump())
            {
                movement.ExitWallrun();
                return MovementState.WallJumping;
            }
            if (!movement.CanWallRun())
            {
                movement.ExitWallrun();
                return MovementState.Falling;
            }
            if (movement.CanDash())
            {
                movement.ExitWallrun();
                return MovementState.Dashing;
            }

            Vector3 projectOnPlane = Vector3.ProjectOnPlane(movement.StateVelocity, movement.WallNormal);

            if (projectOnPlane.sqrMagnitude < minWallrunSpeed * minWallrunSpeed + decelerationThreshold)
            {
                movement.ExitWallrun();
                return MovementState.Falling;
            }

            return State;
        }

        public override (float, float) GetHeight(EntityMovement movement)
        {
            return (movement.BaseCapsuleHeight, movement.BaseHeadHeight);
        }

        public override MovementState State => MovementState.WallRunning;
    }
}