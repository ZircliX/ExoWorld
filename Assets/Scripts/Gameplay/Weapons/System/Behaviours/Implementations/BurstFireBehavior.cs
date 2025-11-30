using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class BurstFireBehavior : IFireBehaviour
    {
        public int ConsecutiveShots { get; private set; }

        public void OnInitialize(Weapon weapon)
        {
            throw new System.NotImplementedException();
        }

        public void OnShootInput(InputAction.CallbackContext context)
        {
            throw new System.NotImplementedException();
        }

        public void Tick(float deltaTime)
        {
            throw new System.NotImplementedException();
        }
    }
}