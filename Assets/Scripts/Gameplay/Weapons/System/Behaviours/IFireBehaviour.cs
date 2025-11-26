using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public interface IFireBehaviour
    {
        void OnInitialize(Weapon weapon);
        void OnShootInput(InputAction.CallbackContext context);
        void Tick(float deltaTime);
    }
}