using System;
using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Cameras;
using OverBang.ExoWorld.Gameplay.Core.Menus;
using OverBang.ExoWorld.Gameplay.Player.PlayerHUD;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Loadout
{
    public class LoadoutTable : MonoBehaviour, IInteractable
    {
        [Header("Weapon Collections")]
        [SerializeField] private Transform uiTarget;
        [SerializeField] private WeaponData[] primaryWeaponData;
        [SerializeField] private WeaponData[] secondaryWeaponData;

        public event Action OnWeaponSelectionRequest;

        public string InteractionText => string.Empty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
        public InteractionType SupportedInteractions => InteractionType.Interact;
        Vector3 IInteractable.UIPosition => uiTarget.position;

        public void Interact(PlayerInteraction playerInteraction)
        {
            StartLoadoutSelection();
        }

        public void StartLoadoutSelection()
        {
            CanInteract = false;
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.LoadoutCamera);
            HUD.Instance.ChangeHudState(true);
            OnWeaponSelectionRequest?.Invoke();
        }

        public void ConfirmSelection(WeaponData primaryData, WeaponData secondaryData = null)
        {
            // Assign default weapons
            if ((primaryData == null && PlayerLoadout.Loadout.primaryWeapon == null) || primaryData == null)
                primaryData = primaryWeaponData[0];
            if ((secondaryData == null && PlayerLoadout.Loadout.secondaryWeapon == null) || secondaryData == null)
                secondaryData = secondaryWeaponData[0];
            
            HUD.Instance.ChangeHudState(false);
            PlayerLoadout.SetWeapons(primaryData, secondaryData);
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.PlayerViewCamera);
            CanInteract = true;
        }
    }
}