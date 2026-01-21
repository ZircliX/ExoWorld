using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay
{
    public interface IReloadBehaviour
    {
        void OnInitialize(Weapon weapon);
        void OnReloadInput(InputAction.CallbackContext context);
        Awaitable Reload();
        void Tick(float dt);
    }
}