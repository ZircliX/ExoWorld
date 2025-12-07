using System.Collections.Generic;
using KBCore.Refs;
using OverBang.GameName.Core;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace OverBang.GameName.Gameplay
{
    [RequireComponent(typeof(DamageableAndHealableComponent))]
    public class Enemy : NetworkBehaviour, IPoolInstanceListener, IDamageSource
    {
        [field : Header("Datas")]
        
        [field : SerializeField, Self, HideInInspector] public NetworkObject EnemyNetworkObject { get; private set; }
        [field : SerializeField] public NavMeshAgent Agent { get; private set; }
        
        [field : Self] public DamageableAndHealableComponent DahComponent { get; private set; }
        
        [SerializeField] public EnemyData enemyData;
        public IPool Pool { get; protected set; }
        
        public DamageInfo DamageInfo => enemyData.DamageInfo;
        
        [field : Header("Informations :")]
        
        [field : Space(15f)]
        [field : SerializeField] public Transform enemyModelContainer { get; private set; }
        
        [field : Header("Patrol Parameters :")]
        [field : Space(15f)]
        
        [field : SerializeField] public SphereCollider TriggerSphere { get; private set; }
        [field : SerializeField] public float TriggerRadius { get; private set; }

        [field: SerializeField] public float patrolRadius { get; private set; } = 10f;

        [field : SerializeField] public float stopDistance { get; private set; } = 1f;
        
        private List<Transform> currentPlayersInRange;
        
        private Vector3 targetPoint;
        private bool isPatrol;
        private bool isTargetting;

        #region Initialization
        
        private void OnValidate()
        {
            this.ValidateRefs();

            DahComponent.OnDamaged += Damaged;
            TriggerSphere.radius = TriggerRadius;
        }

        private void Damaged()
        {
            Debug.Log($"Remaining Health : {DahComponent.Health}");
            
            if (DahComponent.Health <= 0)
            {
                gameObject.Despawn();
            }
        }

        public void Initialize(string enemyDataId)
        {
            SetEnemyModelRpc(enemyDataId);
            
            if (IsOwner)
            {
                currentPlayersInRange = new List<Transform>();
                EnemyManager.Instance.Register(this);
            }
        }
        
        [Rpc(SendTo.Everyone)]
        private void SetEnemyModelRpc(string enemyDataId)
        {
            if (enemyDataId.TryGetAssetByID(out enemyData))
            {
                GameObject playerModel = Instantiate(enemyData.ModelPrefab, enemyModelContainer);
            }
        }
        
        #endregion Initialization

        private void Update()
        {
            if (!IsOwner) return;
            
            if (currentPlayersInRange.Count <= 0 && !Agent.pathPending && Agent.remainingDistance <= stopDistance)
            {
                ChooseNewDestination();
            }
            else if (currentPlayersInRange.Count > 0)
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
        
        private void OnTriggerEnter(Collider other)
        {
            if (!IsOwner) return;
            
            if (other.CompareTag("Player") || other.CompareTag("LocalPlayer"))
            {   
                currentPlayersInRange.Add(other.transform);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!IsOwner) return;
            
            if (other.CompareTag("Player") || other.CompareTag("LocalPlayer"))
            {
                currentPlayersInRange.Remove(other.transform);
            }
        }
        
        #endregion detection
        

        public void OnDeath()
        {
            EnemyManager.Instance.Unregister(this);
            gameObject.Despawn();
        }
        
        
        #region Pooling Implementation
        
        public void OnSpawn(IPool pool)
        {
            Pool = pool;
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