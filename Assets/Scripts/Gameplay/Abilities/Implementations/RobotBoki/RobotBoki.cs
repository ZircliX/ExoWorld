using System;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Targeting;
using UnityEngine;
using UnityEngine.AI;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    public class RobotBoki : MonoBehaviour, ITargetable
    {
        [SerializeField, Self] private NavMeshAgent agent;

        public event Action OnExploded;
        
        private RobotBokiData data;
        private IRobotBehaviour robotBehaviour;
        
        private float currentLifeTime;

        private void OnValidate() => this.ValidateRefs();

        public void Initialize(RobotBokiData data, IRobotBehaviour robotBehaviour)
        {
            this.data = data;
            this.robotBehaviour = robotBehaviour;
            currentLifeTime = 0;

            this.robotBehaviour.ExplosionStrategy.OnExploded += OnExplosionStrategyExploded;
            
            robotBehaviour.Initialize(this, agent, () =>
            {
                Collider[] colliders = Physics.OverlapSphere(
                    transform.position,
                    robotBehaviour.StrategyData.ExplosionRadius,
                    GameMetrics.Global.HittableLayers,
                    QueryTriggerInteraction.Collide);

                return colliders;
            });
            
            agent.speed = data.Speed;
        }

        internal void Tick(float deltaTime)
        {
            currentLifeTime += deltaTime;

            if (currentLifeTime >= data.MaxLifeTime)
            {
                Explode();
            }

            robotBehaviour.Tick(deltaTime);
        }

        private void Explode()
        {
            robotBehaviour.Explode();
        }
        
        private void OnExplosionStrategyExploded(bool terminated)
        {
            if (!terminated) 
                return;
            Debug.Log("Destroy RobotBoki");
            
            robotBehaviour.ExplosionStrategy.OnExploded -= OnExplosionStrategyExploded;
            robotBehaviour.Dispose();
            OnExploded?.Invoke();
        }
        
        public event Action<bool> OnTargetableChanged;
        public Transform Transform => transform;
        public TargetPriority Priority { get; private set; } = TargetPriority.VeryHigh;
        public bool IsTargetable { get; private set; }
        
        public void SetTargetable(bool state)
        {
            IsTargetable = state;
            OnTargetableChanged?.Invoke(state);
        }
    }
}