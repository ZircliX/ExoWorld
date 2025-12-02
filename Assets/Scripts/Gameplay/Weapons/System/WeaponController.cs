using OverBang.GameName.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class WeaponController : MonoBehaviour, IPlayerComponent
    {
        public Loadout Loadout { get; private set; }
        private Camera cam;
        public Camera InteractionCamera
        {
            get
            {
                if (cam == null) cam = Camera.main;
                return cam;
            }
        }
        public PlayerController Controller { get; set; }

        private Weapon primaryWeapon;
        private Weapon secondaryWeapon;
        private Weapon currentWeapon;
        
        private void OnEnable()
        {
            PlayerLoadout.OnLoadoutChanged += ReloadLoadout;
        }

        private void OnDisable()
        {
            PlayerLoadout.OnLoadoutChanged -= ReloadLoadout;
        }

        private void ReloadLoadout()
        {
            OnSync(null, null);
        }

        public void OnSync(CharacterData data, Animator animator)
        {
            Loadout = PlayerLoadout.Loadout;
            if (Loadout.primaryWeapon == null && Loadout.secondaryWeapon == null)
                return;
            
            primaryWeapon = Instantiate(Loadout.primaryWeapon.WeaponPrefab, transform);
            secondaryWeapon = Instantiate(Loadout.secondaryWeapon.WeaponPrefab, transform);
            primaryWeapon.gameObject.SetActive(false);
            secondaryWeapon.gameObject.SetActive(false);
            
            primaryWeapon.Initialize(Loadout.primaryWeapon, InteractionCamera);
            secondaryWeapon.Initialize(Loadout.secondaryWeapon, InteractionCamera);

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
            if (!context.performed || currentWeapon == null)
                return;

            currentWeapon.gameObject.SetActive(false);
            currentWeapon = currentWeapon == primaryWeapon ? secondaryWeapon : primaryWeapon;
            currentWeapon.gameObject.SetActive(true);
        }
    }
}