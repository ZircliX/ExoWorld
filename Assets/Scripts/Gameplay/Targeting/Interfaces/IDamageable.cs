using OverBang.ExoWorld.Core;

namespace OverBang.ExoWorld.Gameplay
{
    public interface IDamageable
    {
        bool IsInvincible { get; set; }
        void TakeDamage(DamageInfo damage);
    }
}