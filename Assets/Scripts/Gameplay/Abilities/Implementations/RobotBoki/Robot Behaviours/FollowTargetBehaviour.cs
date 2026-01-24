using System;
using UnityEngine;
using UnityEngine.AI;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class FollowTargetBehaviour : BaseRobotBehaviour
    {
        private ITargetable currentTarget;
        private float targetSearchTimer;
        private const float TARGET_SEARCH_INTERVAL = 0.5f; // Search for targets every 0.5 seconds
        private const float EXPLOSION_DISTANCE = 2f;
        private const float NAVMESH_SEARCH_RADIUS = 100f;

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

            // Only set destination if we have a valid target
            if (currentTarget != null)
            {
                agent.SetDestination(currentTarget.Transform.position);

                // Check if we've reached the target
                if (!agent.pathPending && agent.remainingDistance < EXPLOSION_DISTANCE)
                {
                    Explode();
                }
            }
        }

        private void SearchForTarget()
        {
            if (EnemyManager.Instance.TryGetClosest(agent.transform.position, 
                    StrategyData.DetectionRadius, 
                    out ITargetable closest))
            {
                currentTarget = closest;
            }
            else
            {
                // No target found, set roaming destination
                SetRoamingDestination();
            }
        }

        private void SetRoamingDestination()
        {
            Vector3 forwardTarget = agent.transform.position + agent.transform.forward * NAVMESH_SEARCH_RADIUS;
    
            if (NavMesh.SamplePosition(forwardTarget, out NavMeshHit hit, NAVMESH_SEARCH_RADIUS, NavMesh.AllAreas))
            {
                currentTarget = null; // Clear target to indicate roaming
                agent.SetDestination(hit.position);
            }
            else
            {
                Debug.LogWarning("No valid NavMesh point found for roaming");
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