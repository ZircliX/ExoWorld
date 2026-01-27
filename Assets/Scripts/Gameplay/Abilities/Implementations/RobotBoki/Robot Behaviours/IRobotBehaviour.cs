using System;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;
using UnityEngine.AI;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public interface IRobotBehaviour
    {
        IRobotBokiAbilityStrategyData StrategyData { get; }
        IExplosionStrategy ExplosionStrategy { get; }

        void Initialize(ITargetable robotTarget, NavMeshAgent agent, Func<Collider[]> getOverlapColliders);
        void Tick(float deltaTime);
        void Explode();
        void Dispose();
    }
}