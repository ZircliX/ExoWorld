using System;
using UnityEngine;

namespace OverBang.GameName.Core.Upgrades
{
    [System.Serializable]
    public struct UpgradeData : IUpgrade, IEquatable<UpgradeData>
    {
        [field: SerializeField] public UpgradeTarget Target { get; private set; }
        [field: SerializeField] public UpgradeData[] Requirements { get; private set; }
        [field: SerializeField] public UpgradeValueType ValueType { get; private set; }
        [field: SerializeField] public float Value { get; private set; }

        public float GetValue(float baseValue)
        {
            return ValueType switch
            {
                UpgradeValueType.Flat => baseValue + Value,
                UpgradeValueType.Percent => baseValue * (1f + Value / 100f),
                _ => baseValue
            };
        }

        public bool Equals(UpgradeData other)
        {
            return Target == other.Target && Equals(Requirements, other.Requirements) && ValueType == other.ValueType && Value.Equals(other.Value);
        }

        public override bool Equals(object obj)
        {
            return obj is UpgradeData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)Target, Requirements, (int)ValueType, Value);
        }
    }
}