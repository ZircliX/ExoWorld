namespace OverBang.GameName.Gameplay
{
    public interface IDamageSource
    {
        DamageInfo DamageInfo { get; }

        void Damage(IDamageable damageable);
    }
}