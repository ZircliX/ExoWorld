using System;
using System.Numerics;
using UnityEngine;
using UnityEngine.AI;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public abstract class BaseRobotBehaviour : IRobotBehaviour
    {
        public IRobotBokiAbilityStrategyData StrategyData { get; protected set; }
        public IExplosionStrategy ExplosionStrategy { get; protected set; }

        protected NavMeshAgent agent;
        protected Func<Collider[]> getOverlapColliders;

        protected BaseRobotBehaviour(IRobotBokiAbilityStrategyData strategyData, IExplosionStrategy explosionStrategy)
        {
            StrategyData = strategyData;
            ExplosionStrategy = explosionStrategy;
        }

        public virtual void Initialize(ITargetable robotTarget, NavMeshAgent agent, Func<Collider[]> getOverlapColliders)
        {
            this.agent = agent;
            this.getOverlapColliders = getOverlapColliders;
        }

        public abstract void Tick(float deltaTime);

        public void Explode()
        {
            ExplosionStrategy.Explode(getOverlapColliders);
        }

        public virtual void Dispose() { }
    }
}