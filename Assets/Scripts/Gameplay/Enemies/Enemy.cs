using System;
using KBCore.Refs;
using OverBang.GameName.Core;
using OverBang.Pooling;
using OverBang.Pooling.Resource;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace OverBang.GameName.Gameplay
{
    public class Enemy : NetworkBehaviour, IPoolInstanceListener
    {
        [field : SerializeField, Self] public NetworkObject EnemyNetworkObject { get; private set; }
        [field : SerializeField, Self] public NavMeshAgent NavMeshAgent { get; private set; }
        [field : SerializeField] public Transform enemyModelContainer { get; private set; }
        [SerializeField] public EnemyData enemyData;
        public IPool Pool { get; protected set; }

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void Initialize(string enemyDataId)
        {
            SetEnemyModelRpc(enemyDataId);
            
            if (IsOwner)
            {
                EnemyManager.Instance.Register(this);
            }
        }

        private void Update()
        {
            if (!IsOwner) return;

            //NavMeshAgent.SetDestination();
        }

        public void OnSpawn(IPool pool)
        {
            Pool = pool;
            //TODO : Reset runtime enemies datas 
        }
        
        public void OnDespawn(IPool pool)
        {
        }

        [Rpc(SendTo.Everyone)]
        private void SetEnemyModelRpc(string enemyDataId)
        {
            if (enemyDataId.TryGetAssetByID(out enemyData))
            {
                GameObject playerModel = Instantiate(enemyData.ModelPrefab, enemyModelContainer);
            }
            
        }

        public void OnMove()
        {
            if (!IsOwner) return;
        }

        public void OnDeath()
        {
            EnemyManager.Instance.Unregister(this);
        }
        
        //LOGIQUE DE DEPLACEMENT ET TOUT ET TOUT.

    }
}