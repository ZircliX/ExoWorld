using OverBang.ExoWorld.Core;

namespace OverBang.ExoWorld.Gameplay
{
    public interface IDamageSource
    {
        DamageInfo DamageInfo { get; }

        void Damage(IDamageable damageable);
    }
}