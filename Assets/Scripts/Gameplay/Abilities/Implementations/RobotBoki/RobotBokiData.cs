using DamageNumbersPro;
using OverBang.ExoWorld.Core.Characters;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Abilities
{
    [CreateAssetMenu(fileName = "RobotBoki Ability Data", menuName = "OverBang/Abilities/Robot Boki")]
    public class RobotBokiData : AbilityData
    {
        [field: Space]
        [field: SerializeField] public NetworkObject Prefab { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float MaxLifeTime { get; private set; }
        [field: SerializeField] public DamageNumberMesh DamagePrefab { get; private set; }
    }
}