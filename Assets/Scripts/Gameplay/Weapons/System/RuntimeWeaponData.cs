namespace OverBang.GameName.Gameplay
{
    public class RuntimeWeaponState
    {
        public int CurrentBullets { get; private set; }
        public bool CanShoot { get; private set; }
        public float TimeForNextShot;

        private readonly Weapon weapon;
        
        public RuntimeWeaponState(Weapon weapon)
        {
            this.weapon = weapon;
            CurrentBullets = weapon.WeaponData.MagCapacity;
            CanShoot = true;
        }
        
        public bool TryConsume(ref int amount)
        {
            if (CurrentBullets < amount)
            {
                amount = CurrentBullets;
            }

            if (!CanShoot)
            {
                //Debug.LogWarning($"Could not fire weapon : CanShoot == false");
                return false;
            }

            if (TimeForNextShot > 0f)
            {
                //Debug.LogWarning($"Could not fire weapon : TimeForNextShot is {TimeForNextShot} instead of 0 seconds");
                return false;
            }
            
            CurrentBullets -= amount;
            TimeForNextShot = weapon.WeaponData.FireCooldown;
            return true;
        }

        public void SetBullets(int amount, bool canShoot)
        {
            CanShoot = canShoot;
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