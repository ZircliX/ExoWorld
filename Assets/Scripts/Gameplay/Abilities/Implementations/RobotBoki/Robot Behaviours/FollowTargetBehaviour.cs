using System;
using UnityEngine;
using UnityEngine.AI;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class FollowTargetBehaviour : BaseRobotBehaviour
    {
        private ITargetable currentTarget;
        private float targetSearchTimer;
        private const float TARGET_SEARCH_INTERVAL = 0.5f;
        private const float EXPLOSION_DISTANCE = 2f;
        private const float ROTATION_SPEED = 5f;

        public FollowTargetBehaviour(IRobotBokiAbilityStrategyData strategyData, IExplosionStrategy explosionStrategy)
            : base(strategyData, explosionStrategy) { }

        public override void Initialize(ITargetable robotTarget, NavMeshAgent agent, Func<Collider[]> getOverlapColliders)
        {
            base.Initialize(robotTarget, agent, getOverlapColliders);
            
            robotTarget.SetTargetable(false);
            EnemyManager.Instance.OnEnemyUnregistered += OnEnemyUnregister;
            
            SearchForTarget();
            targetSearchTimer = TARGET_SEARCH_INTERVAL;
        }

        public override void Tick(float deltaTime)
        {
            // Only search for new targets periodically
            targetSearchTimer -= deltaTime;
            if (targetSearchTimer <= 0)
            {
                SearchForTarget();
                targetSearchTimer = TARGET_SEARCH_INTERVAL;
            }

            if (agent == null)
                return;

            // Move in a direction
            if (currentTarget != null)
            {
                // Move towards target
                Vector3 directionToTarget = (currentTarget.Transform.position - agent.transform.position).normalized;
                agent.transform.rotation = Quaternion.LookRotation(directionToTarget);
                agent.Move(directionToTarget * (agent.speed * deltaTime));

                // Check if we've reached the target
                float distanceToTarget = Vector3.Distance(agent.transform.position, currentTarget.Transform.position);
                if (distanceToTarget < EXPLOSION_DISTANCE)
                {
                    Explode();
                }
            }
            else
            {
                // Move straight forward
                agent.Move(agent.transform.forward * (agent.speed * deltaTime));
            }
        }

        private void SearchForTarget()
        {
            if (EnemyManager.Instance.TryGetClosest(agent.transform.position, 
                    StrategyData.DetectionRadius, 
                    out ITargetable closest))
            {
                Debug.DrawLine(agent.transform.position, closest.Transform.position, Color.red, 0.2f);
                currentTarget = closest;
            }
        }

        private void OnEnemyUnregister(Enemy enemy)
        {
            if (ReferenceEquals(enemy, currentTarget))
            {
                currentTarget = null;
                SearchForTarget();
            }
        }

        public override void Dispose()
        {
            EnemyManager.Instance.OnEnemyUnregistered -= OnEnemyUnregister;
        }
    }
}