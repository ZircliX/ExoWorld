using DamageNumbersPro;
using OverBang.ExoWorld.Core.Components;
using OverBang.ExoWorld.Core.Damage;
using OverBang.ExoWorld.Core.Inventory;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    [CreateAssetMenu(fileName = "BulletData", menuName = "OverBang/Weapons/BulletData")]
    public class BulletData : ScriptableObject
    {
        [field: Header("Prefabs :")]
        [field: SerializeField] public NetworkObject BulletPrefab { get; protected set; }
        [field: SerializeField] public ParticleSystemReference HitDecalPrefab { get; protected set; }
        
        [field: Header("Movement Data :")]
        [field: SerializeField] public float BulletSpeed { get; protected set; }
        [field: SerializeField] public float BulletLifeTime { get; protected set; } = 10f;
        [field: SerializeField] public int Penetration { get; protected set; }
        
        [field: Header("Damage Data :")]
        [field: SerializeField] public DamageData Damage { get; protected set; }
        [field: SerializeField] public DamageNumberMesh DamagePrefab { get; protected set; }
        
        [field: Header("Item Data :")]
        [field: SerializeField] public ScriptableItemData ItemData { get; protected set; }
    }
}
