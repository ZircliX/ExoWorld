using System;
using KBCore.Refs;
using OverBang.ExoWorld.Core;
using UnityEngine;
using UnityEngine.AI;

namespace OverBang.ExoWorld.Gameplay
{
    public class RobotBoki : MonoBehaviour, ITargetable
    {
        [SerializeField, Self] private Rigidbody rb;
        [SerializeField, Self] private NavMeshAgent agent;
        
        private RobotBokiData data;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void Initialize(RobotBokiData data)
        {
            this.data = data;
            
            if (EnemyManager.Instance.TryGetClosest(transform.position, out ITargetable closest))
            {
                agent.SetDestination(closest.Transform.position);
            }
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