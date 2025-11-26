using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class SemiAutoFireBehavior : IFireBehaviour
    {
        private Weapon weapon;
            
        public void OnInitialize(Weapon weapon)
        {
            this.weapon = weapon;
        }

        public void OnShootInput(InputAction.CallbackContext context)
        {
            if (!context.performed || weapon == null)
                return;

            WeaponData data = weapon.WeaponData;
            RuntimeWeaponState state = weapon.State;

            if (!state.TryConsume(data.BulletPerShot))
                return;

            for (int i = 0; i < data.BulletPerShot; i++)
            {
                weapon.Fire();
            }
        }

        public void Tick(float deltaTime) { }
    }
}