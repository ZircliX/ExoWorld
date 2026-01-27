using System;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;
using UnityEngine.AI;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class ForwardBehaviour : BaseRobotBehaviour
    {
        private Vector3 cachedForwardDirection;
        private const float DIRECTION_UPDATE_INTERVAL = 0.2f; // Update direction every 0.2 seconds
        private float directionUpdateTimer;

        public ForwardBehaviour(IRobotBokiAbilityStrategyData strategyData, IExplosionStrategy explosionStrategy)
            : base(strategyData, explosionStrategy) { }

        public override void Initialize(ITargetable robotTarget, NavMeshAgent agent, Func<Collider[]> getOverlapColliders)
        {
            base.Initialize(robotTarget, agent, getOverlapColliders);
            robotTarget.SetTargetable(true);
            
            cachedForwardDirection = agent.transform.forward;
            directionUpdateTimer = DIRECTION_UPDATE_INTERVAL;
        }

        public override void Tick(float deltaTime)
        {
            // Update cached direction periodically instead of every frame
            directionUpdateTimer -= deltaTime;
            if (directionUpdateTimer <= 0)
            {
                cachedForwardDirection = agent.transform.forward;
                directionUpdateTimer = DIRECTION_UPDATE_INTERVAL;
            }

            // Move in the forward direction
            Vector3 movement = cachedForwardDirection * (Time.deltaTime * agent.speed);
            agent.Move(movement);
        }
    }
}