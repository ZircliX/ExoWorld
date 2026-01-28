using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public interface IReloadBehaviour
    {
        bool IsReloading { get; }
        
        void OnInitialize(Weapon weapon);
        void OnReloadInput(InputAction.CallbackContext context);
        Awaitable Reload();
        void Tick(float dt);
    }
}