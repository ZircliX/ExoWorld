using System;
using KBCore.Refs;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class Enemy : MonoBehaviour
    {
        [field : SerializeField, Self] public NetworkObject NetworkObject { get; private set; }
        [SerializeField] public EnemyData enemyData;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void Initialize(EnemyData data)
        {
            enemyData = data;
        }

        public void OnMove()
        {
            
        }

        public void OnDeath()
        {
            
        }
        
        //LOGIQUE DE DEPLACEMENT ET TOUT ET TOUT.
        
    }
}