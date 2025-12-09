using System;
using OverBang.GameName.Core;
using OverBang.GameName.Gameplay.Interface;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class UpgradeTable : MonoBehaviour, IInteractable
    {
        [SerializeField] private CanvasGroup upgradeGroup;
        public string InteractionText => string.Empty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
        public void Interact(PlayerInteraction playerInteraction)
        {
            StartUpgradeSelection();
        }

        public void StartUpgradeSelection()
        {
            upgradeGroup.alpha = 1;
            upgradeGroup.blocksRaycasts = true;
            upgradeGroup.interactable = true;
            CanInteract = false;
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.UpgradeCamera);
            HUD.Instance.ChangeHudState(true);
            UpgradeManager.Instance.RefreshTable();
        }
        
        public void StopUpgradeSelection()
        {
            upgradeGroup.alpha = 0;
            upgradeGroup.blocksRaycasts = false;
            upgradeGroup.interactable = false;
            CanInteract = true;
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.PlayerViewCamera);
            HUD.Instance.ChangeHudState(false);
            
        }
    }
}