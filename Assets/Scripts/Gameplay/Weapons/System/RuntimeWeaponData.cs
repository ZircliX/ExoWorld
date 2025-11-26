namespace OverBang.GameName.Gameplay
{
    public class RuntimeWeaponState
    {
        public int CurrentBullets { get; private set; }
        public int MaxBullets { get; private set; }
        public bool CanShoot { get; private set; } = true;

        private readonly WeaponData data;
        float timeForNextShot;
        
        public RuntimeWeaponState(WeaponData data)
        {
            this.data = data;
            MaxBullets = data.MagCapacity;
            CurrentBullets = MaxBullets;
        }
        
        public bool TryConsume(int amount)
        {
            if (!CanShoot)
                return false;

            if (timeForNextShot > 0f)
                return false;

            if (CurrentBullets < amount)
                return false;

            CurrentBullets -= amount;
            timeForNextShot = data.ShootingRate;
            return true;
        }

        public void SetBullets(int amount, bool canShoot)
        {
            CanShoot = canShoot;
            CurrentBullets = amount;
        }

        public void Tick(float deltaTime)
        {
            if (timeForNextShot <= 0f)
                return;
            
            timeForNextShot -= deltaTime;
        }
    }
}