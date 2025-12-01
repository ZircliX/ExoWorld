using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class SemiAutoFireBehavior : BaseFireBehaviour
    {
        public override void OnShootInput(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            if (Weapon.State.CurrentBullets <= 0)
            {
                // TODO : Reload when no bullets
            }
            
            HandleFire();
        }

        public override void Tick(float deltaTime)
        {
            if (consecutiveShotsValue > 0f)
                return;
            
            HandleConsecutiveShots();
        }
    }
}