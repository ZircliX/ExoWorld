using OverBang.ExoWorld.Core.Damage;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public interface IDamageable
    {
        void TakeDamage(DamageData damage);
    }
}