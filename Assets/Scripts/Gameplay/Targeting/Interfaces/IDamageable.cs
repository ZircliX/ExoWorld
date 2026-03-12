using OverBang.ExoWorld.Core.Damage;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public interface IDamageable
    {
        Transform DamageTarget { get; }
        void TakeDamage(RuntimeDamageData damage);
    }
}