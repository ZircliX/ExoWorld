using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public interface IReloadBehaviour
    {
        void OnInitialize(Weapon weapon);
        void OnReloadInput(InputAction.CallbackContext context);
        void Tick(float dt);
    }
}