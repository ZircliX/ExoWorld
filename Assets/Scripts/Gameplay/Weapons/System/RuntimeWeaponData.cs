namespace OverBang.GameName.Gameplay
{
    public class RuntimeWeaponState
    {
        public int CurrentBullets { get; private set; }
        public bool CanShoot => !IsReloading && CurrentBullets > 0 && TimeForNextShot <= 0f;
        public bool IsReloading  { get; private set; }
        public float TimeForNextShot;

        private readonly Weapon weapon;
        
        public RuntimeWeaponState(Weapon weapon)
        {
            this.weapon = weapon;
            CurrentBullets = weapon.WeaponData.MagCapacity + weapon.WeaponData.UpgradeMagCap;
        }
        
        public bool TryConsume(ref int amount)
        {
            if (!CanShoot)
            {
                //Debug.LogWarning($"Could not fire weapon : CanShoot == false");
                return false;
            }
            
            if (CurrentBullets < amount)
            {
                amount = CurrentBullets;
            }
            
            CurrentBullets -= amount;
            TimeForNextShot = weapon.WeaponData.FireCooldown;
            return true;
        }

        public void SetBullets(int amount, bool reloading)
        {
            IsReloading = reloading;
            CurrentBullets = amount;
        }

        public void Tick(float deltaTime)
        {
            if (TimeForNextShot <= 0f)
                return;
            
            TimeForNextShot -= deltaTime;
        }
    }
}