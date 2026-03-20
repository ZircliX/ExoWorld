using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class SemiAutoFireBehavior : BaseFireBehaviour
    {
        protected override void OnWeaponSetCurrent(bool val)
        {
            if (!val)
            {
                ConsecutiveShots = 0;
                consecutiveShotsValue = 0f;
            }
        }

        public override void OnShootInput(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;
            
            HandleFire();
        }

        public override void Tick(float deltaTime)
        {
            if (consecutiveShotsValue <= 0f)
                return;
            
            HandleConsecutiveShots();
        }
    }
}