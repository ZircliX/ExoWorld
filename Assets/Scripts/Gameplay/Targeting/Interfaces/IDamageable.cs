using System;

namespace OverBang.GameName.Gameplay
{
    public interface IDamageable
    {
        event Action OnDamaged;
        float Health { get; }
        float MaxHealth { get; }
        bool IsAlive { get; }
        bool IsInvincible { get; set; }
        void TakeDamage(DamageInfo damage);
    }
}