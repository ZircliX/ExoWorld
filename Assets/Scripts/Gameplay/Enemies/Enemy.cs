using System;
using System.Collections.Generic;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Enemies;
using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Abilities;
using OverBang.ExoWorld.Gameplay.Targeting;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    [RequireComponent(typeof(DamageableAndHealableComponent))]
    public class Enemy : NetworkBehaviour, IPoolInstanceListener, IDamageSource, ITargetable, ISlowable
    {
        [field : Header("Datas")]
        [field : SerializeField, Self, HideInInspector] public NetworkObject EnemyNetworkObject { get; private set; }
        [field : SerializeField] public NavMeshAgent Agent { get; private set; }
        
        [field : SerializeField, Self] public DamageableAndHealableComponent DahComponent { get; private set; }
        
        [SerializeField] public EnemyData enemyData;
        public IPool Pool { get; protected set; }
        
        public DamageInfo DamageInfo => enemyData.DamageInfo;
        
        [field : Header("Informations :")]
        [field : SerializeField] public Transform enemyModelContainer { get; private set; }
        
        [field : Header("Patrol Parameters :")]
        [field : SerializeField] public DetectionArea FocusDetectionArea { get; private set; }
        [field : SerializeField] public DetectionArea AttackDetectionArea { get; private set; }

        [field: SerializeField] public float patrolRadius { get; private set; } = 10f;

        private List<Transform> currentPlayersInRange;

        private EnemyAnimator enemyAnimator;
        [SerializeField, Self] private CapsuleCollider collider; 
        private Vector3 targetPoint;
        private bool isPatrol;
        private bool isAttacking;
        
        private const string IsPatrol = "isWalking";
        private const string IsPunching = "PlayerDetected";

        #region Initialization
        
        private void OnValidate()
        {
            this.ValidateRefs();
        }
        
        private void OnEnable()
        {
            DahComponent.OnHealthChanged += Damaged;
            FocusDetectionArea.OnEnter += OnEntered;
            FocusDetectionArea.OnExit += OnExited;

            AttackDetectionArea.OnEnter += OnAttackEnter;
            AttackDetectionArea.OnExit += OnAttackExit;
        }

        private void OnDisable()
        {
            DahComponent.OnHealthChanged -= Damaged;
            FocusDetectionArea.OnEnter -= OnEntered;
            FocusDetectionArea.OnExit -= OnExited;

            AttackDetectionArea.OnEnter -= OnAttackEnter;
            AttackDetectionArea.OnExit -= OnAttackExit;
        }

        private void Damaged(float previousHealth, float currentHealth)
        {
            //Debug.Log($"Remaining Health : {DahComponent.Health}");
            if (currentHealth <= 0)
            {
                OnDeath();
            }
        }

        public void Initialize(string enemyDataId)
        {
            SetEnemyModelRpc(enemyDataId);
            collider.enabled = true;
            enemyAnimator.Ragdoll(false);

            if (IsOwner)
            {
                Agent.speed = enemyData.BaseSpeed;
                FocusDetectionArea.SetRequireInterface<ITargetable>();
                FocusDetectionArea.SetAllowedTags("Player", "LocalPlayer");
                AttackDetectionArea.SetRequireInterface<ITargetable>();
                AttackDetectionArea.SetAllowedTags("Player", "LocalPlayer");
                
                // TODO : Setup Health based on enemy data
                currentPlayersInRange = new List<Transform>();
                EnemyManager.Instance.Register(this);
            }
        }
        
        [Rpc(SendTo.Everyone)]
        private void SetEnemyModelRpc(string enemyDataId)
        {
            if (enemyDataId.TryGetAssetByID(out enemyData))
            {
                DahComponent.SetHealth(enemyData.BaseHealth);
                
                GameObject enemyModel = Instantiate(enemyData.ModelPrefab, enemyModelContainer);
                if (enemyModel.TryGetComponent(out EnemyAnimator animator))
                {
                    enemyAnimator = animator;
                    isPatrol = true;
                    enemyAnimator.SetBool(IsPatrol, isPatrol);
                }
            }
        }
        
        #endregion Initialization

        private void Update()
        {
            if (!IsOwner) return;
            if (!Agent.enabled) return;
            
            if (currentPlayersInRange.Count <= 0 && !Agent.pathPending && Agent.remainingDistance <= Agent.stoppingDistance && !isAttacking)
            {
                ChooseNewDestination();
            }
            else if (currentPlayersInRange.Count > 0 && !isAttacking)
            {
                Vector3 target = GetClosestPlayer();
                Agent.SetDestination(target);
            }
        }
        
        private void ChooseNewDestination()
        {
            Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
            randomDirection += transform.position;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
            {
                targetPoint = hit.position;
                Agent.SetDestination(targetPoint);
            }
            else
            {
                ChooseNewDestination();
            }
        }

        #region detection
        
        private Vector3 GetClosestPlayer()
        {
            Transform closest = currentPlayersInRange[0];
            if (closest.position == Vector3.zero) return transform.position;
            
            float closestDistance = Vector3.Distance(transform.position, closest.position);
            
            foreach (Transform player in currentPlayersInRange)
            {
                float distance = Vector3.Distance(transform.position, player.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closest = player;
                }
            }

            return closest.position;
        }
        
        private void OnEntered(Collider col, object target)
        {
            if (!IsOwner) return;
            //Debug.Log("player entered !!!!!!!!!");
            currentPlayersInRange.Add(col.transform);
        }

        private void OnExited(Collider col, object target)
        {
            if (!IsOwner) return;
            currentPlayersInRange.Remove(col.transform);
        }
        
        private void OnAttackEnter(Collider col, object target)
        {
            //Debug.Log("player entered, attacking !!!!!!!!!");
            isPatrol = false;
            enemyAnimator.SetBool(IsPatrol, isPatrol);
            
            isAttacking = true;
            enemyAnimator.SetBool(IsPunching,  isAttacking);
        }
        
        private void OnAttackExit(Collider col, object target)
        {
            //Debug.Log("player leaving..., chasing !!!!!!!!!!");
            isAttacking = false;
            enemyAnimator.SetBool(IsPunching,  isAttacking);
            
            isPatrol = true;
            enemyAnimator.SetBool(IsPatrol, isPatrol);
        }
        
        #endregion detection
        

        protected void OnDeath()
        {
            collider.enabled = false;
            Agent.enabled = false;
            enemyAnimator.Ragdoll(true);
            Invoke(nameof(WaitUntilRagdoll), 2f);
        }

        private void WaitUntilRagdoll()
        {
            EnemyManager.Instance.Unregister(this);
            
            if (IsOwner)
            {
                OnDeathOwner();
            }
            else
            {
                gameObject.SetActive(false);
                OnDeathRpc();
            }
        }

        [Rpc(SendTo.Owner)]
        private void OnDeathRpc()
        {
            OnDeathOwner();
        }

        private void OnDeathOwner()
        {
            EnemyNetworkObject.Despawn();
        }
        
        
        #region Pooling Implementation
        
        public void OnSpawn(IPool pool)
        {
            Pool = pool;
            //DahComponent.Initialize(enemyData.BaseHealth, 0);
            // TODO : Reset runtime enemies datas 
        }
        
        public void OnDespawn(IPool pool)
        {
            
        }
        
        #endregion Pooling Implementation

        public void Damage(IDamageable damageable)
        {
            
        }
        
        public event Action<bool> OnTargetableChanged;
        public Transform Transform => transform;
        public TargetPriority Priority { get; private set; } = TargetPriority.Medium;
        public bool IsTargetable => isTargetable && Agent.enabled;
        private bool isTargetable = true;
        
        public void SetTargetable(bool state)
        {
            isTargetable = state;
            OnTargetableChanged?.Invoke(state);
        }

        public void ApplySlow(float slowPercentage, float slowDuration)
        {
            Agent.speed *= 1f - slowPercentage;
            Invoke(nameof(RemoveSlow), slowDuration);
        }

        private void RemoveSlow()
        {
            Agent.speed = enemyData.BaseSpeed;
        }
    }
}