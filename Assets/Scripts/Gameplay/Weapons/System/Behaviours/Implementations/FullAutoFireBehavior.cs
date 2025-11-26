using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class FullAutoFireBehavior : IFireBehaviour
    {
        private Weapon weapon;
        private bool isFiring;
        
        public void OnInitialize(Weapon weapon)
        {
            this.weapon = weapon;
        }

        public void OnShootInput(InputAction.CallbackContext context)
        {
            if (context.started)
                isFiring = true;
            else if (context.canceled)
                isFiring = false;
        }

        public void Tick(float deltaTime)
        {
            if (!isFiring || weapon == null)
                return;

            WeaponData data  = weapon.WeaponData;
            RuntimeWeaponState state = weapon.State;

            if (!state.TryConsume(data.BulletPerShot))
                return;

            for (int i = 0; i < data.BulletPerShot; i++)
            {
                weapon.Fire();
            }
        }
    }
}