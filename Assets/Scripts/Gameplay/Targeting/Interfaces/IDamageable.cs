namespace OverBang.GameName.Gameplay
{
    public interface IDamageable
    {
        bool IsInvincible { get; set; }
        void TakeDamage(DamageInfo damage);
    }
}