using OverBang.GameName.Core;
using UnityEngine;
using UnityEngine.InputSystem;

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
            primaryWeapon = Instantiate(Loadout.primaryWeapon.WeaponPrefab, transform);
            secondaryWeapon = Instantiate(Loadout.secondaryWeapon.WeaponPrefab, transform);
            primaryWeapon.gameObject.SetActive(false);
            secondaryWeapon.gameObject.SetActive(false);
            
            primaryWeapon.Initialize(Loadout.primaryWeapon, playerCamera);
            secondaryWeapon.Initialize(Loadout.secondaryWeapon, playerCamera);

            currentWeapon = primaryWeapon;
            currentWeapon.gameObject.SetActive(true);
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

            currentWeapon.gameObject.SetActive(false);
            currentWeapon = currentWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon;
            currentWeapon.gameObject.SetActive(true);
        }
    }
}