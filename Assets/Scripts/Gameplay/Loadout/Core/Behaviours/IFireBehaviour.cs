using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public interface IFireBehaviour
    {
        int ConsecutiveShots { get; }

        void OnInitialize(Weapon weapon);
        void OnShootInput(InputAction.CallbackContext context);
        void Tick(float deltaTime);
    }
}