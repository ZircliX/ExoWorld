using System;
using System.Collections.Generic;
using Helteix.Tools;
using KBCore.Refs;
using OverBang.ExoWorld.Core.Components;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Enemies;
using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Loots;
using OverBang.ExoWorld.Gameplay.Targeting;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace OverBang.ExoWorld.Gameplay.Enemies
{
    public class Enemy : NetworkBehaviour, IPoolInstanceListener, IDamageSource, ITargetable, ISpeedTarget
    {
        [Header("References")]
        [SerializeField, Self] private NetworkObject networkObject;
        [SerializeField, Self] private HealthComponent healthComponent;
        [SerializeField, Self] private CapsuleCollider capsuleCollider;
        [SerializeField] private Transform modelContainer;
        [SerializeField] private NavMeshAgent navMeshAgent;

        [field: Header("Detection")]
        [SerializeField] private DetectionArea focusDetectionArea;
        [SerializeField] private DetectionArea attackDetectionArea;

        public EnemyData EnemyData { get; private set; }
        private EnemyAnimator enemyAnimator;

        // Moving
        private Vector3 targetPoint;
        private bool isPatrol;
        private bool isAttacking;

        // Targets
        private List<ITargetable> currentTargetsInRange;
        private ITargetable currentBestTarget;
        private IDamageable currentDamageableTarget;

        // Constants
        private const string IsPatrolParam  = "isWalking";
        private const string IsPunchingParam = "PlayerDetected";

        // Interfaces
        public DamageData DamageData => EnemyData.DamageData;
        public TargetPriority Priority { get; private set; } = TargetPriority.Medium;
        public bool IsTargetable => isTargetable && healthComponent.IsAlive;
        private bool isTargetable = true;
        public event Action<bool> OnTargeted;

        private void OnValidate() => this.ValidateRefs();

        private void OnEnable()
        {
            healthComponent.OnHealthChanged += OnHealthChanged;
            focusDetectionArea.OnEnter += OnEntered;
            focusDetectionArea.OnExit  += OnExited;
            attackDetectionArea.OnEnter += OnAttackEnter;
            attackDetectionArea.OnExit  += OnAttackExit;
        }

        private void OnDisable()
        {
            healthComponent.OnHealthChanged -= OnHealthChanged;
            focusDetectionArea.OnEnter -= OnEntered;
            focusDetectionArea.OnExit  -= OnExited;
            attackDetectionArea.OnEnter -= OnAttackEnter;
            attackDetectionArea.OnExit  -= OnAttackExit;
        }

        public void Initialize(string enemyDataId)
        {
            SetEnemyModelRpc(enemyDataId);
            capsuleCollider.enabled = true;
            enemyAnimator.Ragdoll(false);

            if (IsOwner)
            {
                navMeshAgent.speed = EnemyData.BaseSpeed;
                focusDetectionArea.SetRequireInterface<ITargetable>();
                focusDetectionArea.GetCollider<SphereCollider>().radius = EnemyData.TriggerDetectionRadius;
                attackDetectionArea.SetRequireInterface<IDamageable>();
                attackDetectionArea.GetCollider<SphereCollider>().radius = EnemyData.AttackDetectionRadius;

                currentTargetsInRange = new List<ITargetable>();
                EnemyManager.Instance.Register(this);
            }
        }

        [Rpc(SendTo.Everyone)]
        private void SetEnemyModelRpc(string enemyDataId)
        {
            if (enemyDataId.TryGetAssetByID(out EnemyData enemyData))
            {
                EnemyData = enemyData;
                healthComponent.SetHealth(EnemyData.BaseHealth);

                GameObject enemyModel = Instantiate(EnemyData.ModelPrefab, modelContainer);
                if (enemyModel.TryGetComponent(out EnemyAnimator animator))
                {
                    enemyAnimator = animator;
                    isPatrol = true;
                    enemyAnimator.SetBool(IsPatrolParam, isPatrol);
                }
            }
        }

        private void Update()
        {
            if (!IsOwner) return;
            if (!navMeshAgent.enabled) return;

            if (currentTargetsInRange.Count <= 0 && !navMeshAgent.pathPending
                && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance
                && !isAttacking)
            {
                ChooseNewDestination();
            }
            else if (currentTargetsInRange.Count > 0 && !isAttacking)
            {
                // On suit la best target déjà calculée — recalcul seulement si elle bouge
                if (currentBestTarget != null)
                    navMeshAgent.SetDestination(currentBestTarget.transform.position);
            }
        }

        private void ChooseNewDestination()
        {
            Vector3 randomPoint = Random.insideUnitSphere * EnemyData.PatrolRadius + transform.position;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, EnemyData.PatrolRadius, NavMesh.AllAreas))
            {
                targetPoint = hit.position;
                navMeshAgent.SetDestination(targetPoint);
            }
            else
            {
                ChooseNewDestination();
            }
        }
        
        /// <summary>
        /// Réévalue la meilleure cible parmi celles en range.
        /// Appelé à chaque entrée/sortie et sur OnTargeted.
        /// </summary>
        private void ReevaluateBestTarget()
        {
            if (!IsOwner || !navMeshAgent.isOnNavMesh) return;

            ITargetable best = null;
            float bestScore = float.MaxValue;

            foreach (ITargetable target in currentTargetsInRange)
            {
                if (!target.IsTargetable) continue;

                float distance = Vector3.Distance(transform.position, target.transform.position);
                // Plus la priorité est haute, plus on divise la distance → favorise la cible prioritaire
                float score = distance / (1 + (int)target.Priority);

                if (score < bestScore)
                {
                    bestScore = score;
                    best = target;
                }
            }

            if (best == currentBestTarget) return;
            
            if (currentBestTarget != null)
                currentBestTarget.OnTargeted -= OnBestTargetStateChanged;

            currentBestTarget = best;

            if (currentBestTarget != null)
            {
                currentBestTarget.OnTargeted += OnBestTargetStateChanged;
                currentBestTarget.SetTargetable(true);
                navMeshAgent.SetDestination(currentBestTarget.transform.position);
            }
        }

        /// <summary>
        /// Callback : la best target a changé d'état (IsTargetable ou priorité).
        /// </summary>
        private void OnBestTargetStateChanged(bool newState)
        {
            // La cible n'est plus valide → on recalcule
            if (!newState) ReevaluateBestTarget();
        }

        private void OnEntered(Collider col, object target)
        {
            if (!IsOwner) return;
            if (target is not ITargetable targetable) return;

            currentTargetsInRange.Add(targetable);
            // On écoute immédiatement les changements de cette cible
            targetable.OnTargeted += OnAnyTargetStateChanged;
            ReevaluateBestTarget();
        }

        private void OnExited(Collider col, object target)
        {
            if (!IsOwner) return;
            if (target is not ITargetable targetable) return;

            targetable.OnTargeted -= OnAnyTargetStateChanged;
            currentTargetsInRange.Remove(targetable);

            if (currentBestTarget == targetable)
            {
                currentBestTarget.OnTargeted -= OnBestTargetStateChanged;
                currentBestTarget = null;
            }

            ReevaluateBestTarget();
        }

        /// <summary>
        /// N'importe quelle cible en range a changé d'état → recalcul.
        /// </summary>
        private void OnAnyTargetStateChanged(bool _) => ReevaluateBestTarget();

        private void OnAttackEnter(Collider col, object target)
        {
            if (!IsOwner) return;

            currentDamageableTarget = target as IDamageable;
            const float attackOffset = 1.1f + 0.25f;

            isPatrol = false;
            enemyAnimator.SetBool(IsPatrolParam, isPatrol);

            isAttacking = true;
            enemyAnimator.SetBool(IsPunchingParam, isAttacking);

            _ = HandleAttack(attackOffset);
        }

        private void OnAttackExit(Collider col, object target)
        {
            if (!IsOwner) return;

            isAttacking = false;
            enemyAnimator.SetBool(IsPunchingParam, isAttacking);

            isPatrol = true;
            enemyAnimator.SetBool(IsPatrolParam, isPatrol);
        }

        private async Awaitable HandleAttack(float attackOffset)
        {
            while (isAttacking)
            {
                await Awaitable.WaitForSecondsAsync(attackOffset);
                Damage(currentDamageableTarget);
                await Awaitable.WaitForSecondsAsync(3.833f - attackOffset);
            }
        }

        private void OnHealthChanged(float previousHealth, float currentHealth, float max)
        {
            if (currentHealth <= 0) OnDeath();
        }

        private void OnDeath()
        {
            isAttacking = false;
            capsuleCollider.enabled = false;
            navMeshAgent.enabled = false;

            // Nettoyage des subscriptions
            if (currentTargetsInRange != null)
            {
                foreach (ITargetable t in currentTargetsInRange)
                    t.OnTargeted -= OnAnyTargetStateChanged;
            }

            if (currentBestTarget != null)
            {
                currentBestTarget.OnTargeted -= OnBestTargetStateChanged;
                currentBestTarget = null;
            }

            RagdollRpc();
            EnemyManager.Instance.Unregister(this);
            EnemyData.LootTable.GetDrop(transform.position, transform.rotation);
            Invoke(nameof(WaitUntilRagdoll), EnemyData.RagdollDuration);
        }

        [Rpc(SendTo.Everyone)]
        private void RagdollRpc()
        {
            enemyAnimator.Ragdoll(true);
        }

        private void WaitUntilRagdoll()
        {
            if (IsOwner) OnDeathOwner();
            else
            {
                gameObject.SetActive(false);
                OnDeathRpc();
            }
        }

        [Rpc(SendTo.Owner)]
        private void OnDeathRpc() => OnDeathOwner();

        private void OnDeathOwner() => networkObject.Despawn();

        public void OnSpawn(IPool pool) => navMeshAgent.enabled = true;
        public void OnDespawn(IPool pool) => modelContainer.ClearChildren();

        public void Damage(IDamageable damageable)
        {
            damageable.TakeDamage(DamageData.GetRuntimeDamage());
        }

        public void SetTargetable(bool state)
        {
            isTargetable = state;
            OnTargeted?.Invoke(state);
        }

        public void ApplySpeed(float speedPercentage, float duration, string effectId)
        {
            navMeshAgent.speed *= 1f - speedPercentage;
            Invoke(nameof(RemoveSlow), duration);
        }

        private void RemoveSlow() => navMeshAgent.speed = EnemyData.BaseSpeed;
    }
}