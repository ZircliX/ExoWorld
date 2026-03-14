using System;
using System.Collections.Generic;
using System.Threading;
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
    public class Enemy : NetworkBehaviour, IDamageSource, ITargetable, ISpeedTarget
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
        private CancellationTokenSource attackCts;

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

        [Rpc(SendTo.Everyone)]
        public void InitializeRpc(string enemyDataId)
        {
            SetEnemyModelRpc(enemyDataId);
            
            navMeshAgent.enabled = true;
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
                else
                    ReevaluateBestTarget();
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
            else
            {
                // No valid target → resume patrol
                ChooseNewDestination();
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

            attackCts?.Cancel();
            attackCts = new CancellationTokenSource();
            HandleAttack(attackOffset, attackCts.Token).Run();
        }

        private void OnAttackExit(Collider col, object target)
        {
            if (!IsOwner) return;

            StopAttack();

            isPatrol = true;
            enemyAnimator.SetBool(IsPatrolParam, isPatrol);
        }
        
        private void StopAttack()
        {
            isAttacking = false;
            enemyAnimator.SetBool(IsPunchingParam, false);
            attackCts?.Cancel();
            attackCts = null;
            currentDamageableTarget = null;
        }

        private async Awaitable HandleAttack(float attackOffset, CancellationToken ct)
        {
            try
            {
                while (isAttacking && !ct.IsCancellationRequested)
                {
                    await Awaitable.WaitForSecondsAsync(attackOffset, ct);

                    if (currentDamageableTarget != null)
                        Damage(currentDamageableTarget);

                    await Awaitable.WaitForSecondsAsync(3.833f - attackOffset, ct);
                }
            }
            catch (OperationCanceledException) { }
        }

        private void OnHealthChanged(float previousHealth, float currentHealth, float max)
        {
            if (currentHealth <= 0 && IsOwner) OnDeathRpc();
        }

        [Rpc(SendTo.Everyone)]
        private void OnDeathRpc()
        {
            StopAttack(); // ← cancels token, clears target, stops loop
            capsuleCollider.enabled = false;
            navMeshAgent.enabled = false;

            ClearTargets();
            EnemyManager.Instance.Unregister(this);
            enemyAnimator.Ragdoll(true);
            Invoke(nameof(WaitUntilRagdoll), EnemyData.RagdollDuration);

            if (IsOwner)
                EnemyData.LootTable.GetDrop(transform.position, transform.rotation);
        }

        private void ClearTargets()
        {
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
        }

        private void WaitUntilRagdoll()
        {
            if (IsOwner)
            {
                networkObject.Despawn();
            }
            else
            {
                gameObject.SetActive(false);
            }
            
            modelContainer.ClearChildren();
        }

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