using OverBang.ExoWorld.Core.Damage;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public interface IDamageSource
    {
        DamageInfo DamageInfo { get; }

        void Damage(IDamageable damageable);
    }
}