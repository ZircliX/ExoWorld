using OverBang.GameName.Core;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityUtils;

namespace OverBang.GameName.Gameplay
{
    public class WeaponController : MonoBehaviour, IPlayerComponent
    {
        [field: SerializeField]
        public Loadout Loadout { get; private set; }
        [field: SerializeField] public Camera playerCamera { get; private set; }
        public PlayerController Controller { get; set; }

        private Weapon primaryWeapon;
        private Weapon secondaryWeapon;
        private Weapon currentWeapon;

        public void OnSync(CharacterData data, Animator animator)
        {
            primaryWeapon = Instantiate(Loadout.primaryWeapon.WeaponPrefab, transform).SetInactive();
            secondaryWeapon = Instantiate(Loadout.secondaryWeapon.WeaponPrefab, transform).SetInactive();

            primaryWeapon.Initialize(Loadout.primaryWeapon, playerCamera);
            secondaryWeapon.Initialize(Loadout.secondaryWeapon, playerCamera);

            currentWeapon = primaryWeapon;
            currentWeapon.SetActive();
        }
        
        public void OnShootInput(InputAction.CallbackContext context)
        {
            currentWeapon?.OnShootInput(context);
        }

        public void OnReloadInput(InputAction.CallbackContext context)
        {
            currentWeapon?.OnReloadInput(context);
        }
        
        public void OnWeaponSwitch(InputAction.CallbackContext context)
        {
            if (!context.performed)
                return;

            currentWeapon.SetInactive();
            currentWeapon = currentWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon;
            currentWeapon.SetActive();
        }
    }
}