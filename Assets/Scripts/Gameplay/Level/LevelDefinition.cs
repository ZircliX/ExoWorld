using System;
using OverBang.Pooling.Resource;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    [System.Serializable]
    public struct LevelDefinition : IEquatable<LevelDefinition>
    {
        [field: SerializeField] public CharacterPoolBinding[] Players { get; private set; }
        [field: SerializeField]	public PoolConfigAsset[] ExtraPools { get; private set; }

        //[field: SerializeField] public EnemyData[] Enemies { get; private set; }

        public bool Equals(LevelDefinition other)
        {
            return Equals(Players, other.Players) && Equals(ExtraPools, other.ExtraPools);
        }

        public override bool Equals(object obj)
        {
            return obj is LevelDefinition other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Players, ExtraPools);
        }
    }
}