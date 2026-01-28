using OverBang.ExoWorld.Core.Damage;

namespace OverBang.ExoWorld.Gameplay.Targeting
{
    public interface IDamageSource
    {
        DamageData DamageData { get; }

        void Damage(IDamageable damageable);
    }
}