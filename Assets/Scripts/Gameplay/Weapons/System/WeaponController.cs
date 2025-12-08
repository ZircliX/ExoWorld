using System;
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

        public Weapon PrimaryWeapon { get; private set; }
        public Weapon SecondaryWeapon { get; private set; }
        public Weapon CurrentWeapon { get; private set; }

        public event Action OnWeaponChanged;
        private PlayerRig playerRig;
        
        private void OnEnable()
        {
            PlayerLoadout.OnLoadoutChanged += RefreshLoadout;
        }

        private void OnDisable()
        {
            PlayerLoadout.OnLoadoutChanged -= RefreshLoadout;
        }

        public void OnSync(PlayerRuntimeContext context)
        {
            playerRig = context.PlayerRig;
            RefreshLoadout();
        }

        private void RefreshLoadout()
        {
            if (PrimaryWeapon != null)
                DestroyImmediate(PrimaryWeapon.gameObject);
            if (SecondaryWeapon != null)
                DestroyImmediate(SecondaryWeapon.gameObject);
            
            Loadout = PlayerLoadout.Loadout;
            if (Loadout.primaryWeapon == null && Loadout.secondaryWeapon == null)
                return;
            
            // Instantiate weapon objects
            PrimaryWeapon = Instantiate(Loadout.primaryWeapon.WeaponPrefab, transform);
            SecondaryWeapon = Instantiate(Loadout.secondaryWeapon.WeaponPrefab, transform);
            
            PrimaryWeapon.gameObject.SetActive(false);
            SecondaryWeapon.gameObject.SetActive(false);

            // Initialize the weapon with loadout
            PrimaryWeapon.Initialize(Loadout.primaryWeapon, InteractionCamera);
            SecondaryWeapon.Initialize(Loadout.secondaryWeapon, InteractionCamera);

            CurrentWeapon = PrimaryWeapon;
            CurrentWeapon.gameObject.SetActive(true);
            
            OnWeaponChanged?.Invoke();
            playerRig.OnWeaponChange(CurrentWeapon.Rig);
        }
        
        public void OnShootInput(InputAction.CallbackContext context)
        {
            CurrentWeapon?.OnShootInput(context);
        }

        public void OnReloadInput(InputAction.CallbackContext context)
        {
            CurrentWeapon?.OnReloadInput(context);
        }
        
        public void OnWeaponSwitch(InputAction.CallbackContext context)
        {
            if (!context.performed || CurrentWeapon == null)
                return;

            CurrentWeapon.gameObject.SetActive(false);
            CurrentWeapon = CurrentWeapon == PrimaryWeapon ? SecondaryWeapon : PrimaryWeapon;
            CurrentWeapon.gameObject.SetActive(true);
            
            OnWeaponChanged?.Invoke();
            playerRig.OnWeaponChange(CurrentWeapon.Rig);
        }
    }
}