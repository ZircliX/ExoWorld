using System;

namespace OverBang.GameName.Gameplay
{
    public interface IDamageable
    {
        event Action OnDamaged;
        float Health { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        void TakeDamage(DamageInfo damage);
    }
}