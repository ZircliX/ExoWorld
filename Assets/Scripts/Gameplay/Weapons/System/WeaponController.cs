using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace OverBang.GameName.Gameplay
{
    public class WeaponController : NetworkBehaviour, IPlayerComponent
    {
        [SerializeField] private WeaponData[] weaponsData;
        public event Action<RigBuilder> OnRigBuilderAccessed;
        
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

        public Weapon CurrentWeapon => CurrentWeaponCategory switch
        {
            WeaponCategory.Primary => PrimaryWeapon,
            WeaponCategory.Secondary => SecondaryWeapon,
            _ => null
        };
        public WeaponCategory CurrentWeaponCategory { get; private set; }
        
        [SerializeField] private Transform weaponHolder;

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

            if (playerRig.TryGetComponent(out RigBuilder builder))
            {
                OnRigBuilderAccessed?.Invoke(builder);
            }
            
            if (!IsOwner)
            {
                weaponHolder = context.playerAnimator.GetBoneTransform(HumanBodyBones.RightHand);
                //weaponHolder.position = weaponHolder.position.Add(z: 0.2f)
            }
            
            RefreshLoadout();
        }

        private void RefreshLoadout()
        {
            if (!IsOwner) return;
            
            Loadout = PlayerLoadout.Loadout;
            if (Loadout.primaryWeapon == null && Loadout.secondaryWeapon == null)
                return;
            
            InstantiateWeaponsRpc(Loadout.primaryWeapon.WeaponName, Loadout.secondaryWeapon.WeaponName);
            SetVisibleWeaponRpc(WeaponCategory.Primary);
        }

        [Rpc(SendTo.Everyone)]
        private void InstantiateWeaponsRpc(string primaryWeaponName, string secondaryWeaponName)
        {
            if (PrimaryWeapon != null)
                DestroyImmediate(PrimaryWeapon.gameObject);
            if (SecondaryWeapon != null)
                DestroyImmediate(SecondaryWeapon.gameObject);
            
            // Instantiate weapon objects
            WeaponData primary = null;
            WeaponData secondary = null;
            for (int i = 0; i < weaponsData.Length; i++)
            {
                if (weaponsData[i].WeaponName == primaryWeaponName)
                    primary = weaponsData[i];
                if (weaponsData[i].WeaponName == secondaryWeaponName)
                    secondary = weaponsData[i];
            }
            
            PrimaryWeapon = Instantiate(primary.Prefab, weaponHolder);
            SecondaryWeapon = Instantiate(secondary.Prefab, weaponHolder);
            
            // Initialize the weapon with loadout
            PrimaryWeapon.Initialize(primary, InteractionCamera);
            SecondaryWeapon.Initialize(secondary, InteractionCamera);
        }

        [Rpc(SendTo.Everyone)]
        private void SetVisibleWeaponRpc(WeaponCategory category)
        {
            PrimaryWeapon.gameObject.SetActive(category == WeaponCategory.Primary);
            SecondaryWeapon.gameObject.SetActive(category == WeaponCategory.Secondary);
            CurrentWeaponCategory = category;
            
            OnWeaponChanged?.Invoke();
            playerRig.OnWeaponChange(CurrentWeapon.Rig);
        }
        
        //INPUTS
        
        public void OnShootInput(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            CurrentWeapon?.OnShootInput(context);
        }

        public void OnReloadInput(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            CurrentWeapon?.OnReloadInput(context);
        }
        
        public void OnWeaponSwitch(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            if (!context.performed || CurrentWeapon == null)
                return;

            SetVisibleWeaponRpc(CurrentWeaponCategory == WeaponCategory.Primary ? WeaponCategory.Secondary : WeaponCategory.Primary);
        }
    }
}