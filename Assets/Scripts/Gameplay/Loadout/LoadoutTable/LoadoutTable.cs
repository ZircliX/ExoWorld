using System;
using OverBang.ExoWorld.Core;
using UnityEngine;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay
{
    public class LoadoutTable : MonoBehaviour, IInteractable
    {
        [Header("Weapon Collections")]
        [SerializeField] private WeaponData[] primaryWeaponData;
        [SerializeField] private WeaponData[] secondaryWeaponData;

        private WeaponData selectedPrimary;
        private WeaponData selectedSecondary;

        public event Action<bool> OnRankSelectionUIRequested;
        public event Action<WeaponRank> OnWeaponSelectionUIRequested;

        public string InteractionText => string.Empty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
        Vector3 IInteractable.UIPosition => transform.position.Add(y: 0.5f);

        public void Interact(PlayerInteraction playerInteraction)
        {
            StartLoadoutSelection();
        }

        public void StartLoadoutSelection()
        {
            CanInteract = false;
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.LoadoutCamera);
            HUD.Instance.ChangeHudState(true);
            OnRankSelectionUIRequested?.Invoke(true);
        } 

        public void OpenPrimary() =>
            OnWeaponSelectionUIRequested?.Invoke(WeaponRank.Primary);

        public void OpenSecondary() =>
            OnWeaponSelectionUIRequested?.Invoke(WeaponRank.Secondary);

        public WeaponData[] GetWeaponList(WeaponRank rank)
            => rank == WeaponRank.Primary ? primaryWeaponData : secondaryWeaponData;

        public void ConfirmSelection()
        {
            // Assign default weapons
            if ((selectedPrimary == null && PlayerLoadout.Loadout.primaryWeapon == null) || selectedPrimary == null)
                selectedPrimary = primaryWeaponData[0];
            if ((selectedSecondary == null && PlayerLoadout.Loadout.secondaryWeapon == null) || selectedSecondary == null)
                selectedSecondary = secondaryWeaponData[0];
            
            OnRankSelectionUIRequested?.Invoke(false);
            HUD.Instance.ChangeHudState(false);
            PlayerLoadout.SetWeapons(selectedPrimary, selectedSecondary);
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.PlayerViewCamera);
            CanInteract = true;
        }

        public void SelectWeapon(WeaponRank rank, WeaponData data)
        {
            if (rank == WeaponRank.Primary)
                selectedPrimary = data;
            else
                selectedSecondary = data;

            OnRankSelectionUIRequested?.Invoke(true);
        }
    }
}