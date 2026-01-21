using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay
{
    public interface IFireBehaviour
    {
        int ConsecutiveShots { get; }

        void OnInitialize(Weapon weapon);
        void OnShootInput(InputAction.CallbackContext context);
        void Tick(float deltaTime);
    }
}