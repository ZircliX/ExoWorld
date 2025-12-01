using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class FullAutoFireBehavior : BaseFireBehaviour
    {
        private bool isFiring;
        
        public override void OnShootInput(InputAction.CallbackContext context)
        {
            if (context.started)
                isFiring = true;
            else if (context.canceled)
                isFiring = false;
        }

        public override void Tick(float deltaTime)
        {
            if (!isFiring && consecutiveShotsValue > 0f)
            {
                HandleConsecutiveShots();
            }

            if (isFiring)
            {
                HandleFire();
            }
        }
    }
}