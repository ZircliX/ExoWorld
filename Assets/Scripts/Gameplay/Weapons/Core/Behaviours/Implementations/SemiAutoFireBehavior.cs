using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class SemiAutoFireBehavior : BaseFireBehaviour
    {
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