using System;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Gameplay.IK_Animation;
using OverBang.ExoWorld.Gameplay.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.InputSystem;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class WeaponController : NetworkBehaviour, IPlayerComponent, IInputReceiver
    {
        [SerializeField] private WeaponData[] weaponsData;
        [SerializeField] private Transform weaponHolder;
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
        public PlayerController Controller { get; private set; }
        private LoadoutController loadoutController;
        public Weapon PrimaryWeapon { get; private set; }
        public Weapon SecondaryWeapon { get; private set; }

        public Weapon CurrentWeapon => CurrentWeaponCategory switch
        {
            WeaponCategory.Primary => PrimaryWeapon,
            WeaponCategory.Secondary => SecondaryWeapon,
            _ => null
        };
        public WeaponCategory CurrentWeaponCategory { get; private set; }
        

        public event Action<Weapon, Weapon> OnWeaponChanged;
        private PlayerRig playerRig;

        public float ShootRateMultiplier { get; private set; } = 1;
        public float DamageMultiplier { get; private set; } = 1;
        
        private void OnEnable()
        {
            PlayerLoadout.OnLoadoutChanged += RefreshLoadout;
        }

        private void OnDisable()
        {
            PlayerLoadout.OnLoadoutChanged -= RefreshLoadout;
        }

        public void SetShootRateMultiplier(float shootRate)
        {
            ShootRateMultiplier = shootRate;
        }

        public void SetDamageMultiplier(float damage)
        {
            DamageMultiplier = damage;
        }

        public void Initialize(LoadoutController loadoutController)
        {
            this.loadoutController = loadoutController;
        }

        public void OnSync(PlayerRuntimeContext context)
        {
            Controller = context.playerController;
            playerRig = context.PlayerRig;

            if (playerRig.TryGetComponent(out RigBuilder builder))
            {
                OnRigBuilderAccessed?.Invoke(builder);
            }
            
            if (!IsOwner)
            {
                Transform arm = context.playerAnimator.GetBoneTransform(HumanBodyBones.RightHand);
                weaponHolder = Instantiate(new GameObject("WeaponHolder"), arm).transform;
                
                weaponHolder.localPosition = new Vector3(-0.067f, 0.212f, 0.034f);
                weaponHolder.localRotation = Quaternion.Euler(-84.267f, 77.452f, 14.835f);
                weaponHolder.localScale = new Vector3(1.50f, 1.50f, 1.50f);
            }
            
            RefreshLoadout();
        }

        private void RefreshLoadout()
        {
            if (!IsOwner) return;
            
            Loadout = PlayerLoadout.Loadout;
            if (Loadout.primaryWeapon == null && Loadout.secondaryWeapon == null)
                return;
            ulong clientID = GamePlayerManager.Instance.GetLocalPlayer().ClientID;
            InstantiateWeaponsRpc(clientID, Loadout.primaryWeapon.WeaponName, Loadout.secondaryWeapon.WeaponName);
            SetVisibleWeaponRpc(WeaponCategory.Primary);
        }

        [Rpc(SendTo.Everyone)]
        private void InstantiateWeaponsRpc(ulong clientID, string primaryWeaponName, string secondaryWeaponName)
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
            
            if (clientID != GamePlayerManager.Instance.GetLocalPlayer().ClientID) return;
            
            // Initialize the weapon with loadout
            PrimaryWeapon.Initialize(primary, InteractionCamera, this);
            SecondaryWeapon.Initialize(secondary, InteractionCamera, this);
        }

        [Rpc(SendTo.Everyone)]
        private void SetVisibleWeaponRpc(WeaponCategory category)
        {
            Weapon previousWeapon = CurrentWeapon;

            PrimaryWeapon.gameObject.SetActive(category == WeaponCategory.Primary);
            SecondaryWeapon.gameObject.SetActive(category == WeaponCategory.Secondary);
            CurrentWeaponCategory = category;

            OnWeaponChanged?.Invoke(previousWeapon, CurrentWeapon);

            if (IsOwner)
                playerRig.OnWeaponChange(CurrentWeapon.Rig);
        } 
        
        #region Inputs

        public void OnLeftInput(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            CurrentWeapon?.OnShootInput(context);
        }

        public void OnRightInput(InputAction.CallbackContext context)
        {
            
        }

        public void OnMiddleDragInput(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            if (!context.performed || CurrentWeapon == null)
                return;

            SetVisibleWeaponRpc(CurrentWeaponCategory == WeaponCategory.Primary ? WeaponCategory.Secondary : WeaponCategory.Primary);
        }

        public void OnRInput(InputAction.CallbackContext context)
        {
            if (!IsOwner) return;
            CurrentWeapon?.OnReloadInput(context);
        }
        
        #endregion
    }
}