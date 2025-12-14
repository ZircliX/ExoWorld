using System;
using System.Collections.Generic;
using KBCore.Refs;
using OverBang.GameName.Core;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace OverBang.GameName.Gameplay
{
    [RequireComponent(typeof(DamageableAndHealableComponent))]
    public class Enemy : NetworkBehaviour, IPoolInstanceListener, IDamageSource
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
        [field : SerializeField] public TriggerComponent GetTriggeredComponent { get; private set; }
        [field : SerializeField] public TriggerComponent AttackTriggerComponent { get; private set; }

        [field: SerializeField] public float patrolRadius { get; private set; } = 10f;

        private List<Transform> currentPlayersInRange;

        private EnemyAnimator enemyAnimator;
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
            DahComponent.OnDamaged += Damaged;
            GetTriggeredComponent.OnEntered += OnEntered;
            GetTriggeredComponent.OnExited += OnExited;
            
            AttackTriggerComponent.OnEntered += OnAttackEnter;
            AttackTriggerComponent.OnExited += OnAttackExit;
        }

        private void OnDisable()
        {
            DahComponent.OnDamaged -= Damaged;
            GetTriggeredComponent.OnEntered -= OnEntered;
            GetTriggeredComponent.OnExited -= OnExited;
            
            AttackTriggerComponent.OnEntered -= OnAttackEnter;
            AttackTriggerComponent.OnExited -= OnAttackExit;
        }

        private void Damaged()
        {
            //Debug.Log($"Remaining Health : {DahComponent.Health}");
            if (DahComponent.Health <= 0)
            {
                OnDeath();
            }
        }

        public void Initialize(string enemyDataId)
        {
            SetEnemyModelRpc(enemyDataId);
            
            if (IsOwner)
            {
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
        
        private void OnEntered(Transform playerTransform)
        {
            if (!IsOwner) return;
            currentPlayersInRange.Add(playerTransform);
        }

        private void OnExited(Transform playerTransform)
        {
            if (!IsOwner) return;
            currentPlayersInRange.Remove(playerTransform);
        }
        
        private void OnAttackEnter(Transform playerTransform)
        {
            Debug.Log("player entered, attacking !!!!!!!!!");
            isPatrol = false;
            enemyAnimator.SetBool(IsPatrol, isPatrol);
            
            isAttacking = true;
            enemyAnimator.SetBool(IsPunching,  isAttacking);
        }
        
        private void OnAttackExit(Transform playerTransform)
        {
            Debug.Log("player leaving..., chasing !!!!!!!!!!");
            isAttacking = false;
            enemyAnimator.SetBool(IsPunching,  isAttacking);
            
            isPatrol = true;
            enemyAnimator.SetBool(IsPatrol, isPatrol);
        }
        
        #endregion detection
        

        protected void OnDeath()
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
            //TODO : Reset runtime enemies datas 
        }
        
        public void OnDespawn(IPool pool)
        {
            
        }
        
        #endregion Pooling Implementation

        public void Damage(IDamageable damageable)
        {
            
        }
    }
}