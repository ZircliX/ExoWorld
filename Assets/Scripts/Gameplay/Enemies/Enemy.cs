using KBCore.Refs;
using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class Enemy : NetworkBehaviour
    {
        [field : SerializeField, Self] public NetworkObject NetworkObject { get; private set; }
        [field : SerializeField] public Transform enemyModelContainer { get; private set; }
        [SerializeField] public EnemyData enemyData;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void Initialize(string enemyDataId)
        {
            SetEnemyModelRpc(enemyDataId);
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
            
        }
        
        //LOGIQUE DE DEPLACEMENT ET TOUT ET TOUT.
        
    }
}