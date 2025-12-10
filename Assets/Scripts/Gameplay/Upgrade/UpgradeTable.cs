using System;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class UpgradeTable : MonoBehaviour, IInteractable
    {
        public string InteractionText => string.Empty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
        
        public event Action<bool> OnUpgradePanelRequest;
        
        public void Interact(PlayerInteraction playerInteraction)
        {
            StartUpgradeSelection();
        }

        public void StartUpgradeSelection()
        {
            CanInteract = false;
            OnUpgradePanelRequest?.Invoke(true);
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.UpgradeCamera);
        }
        
        public void StopUpgradeSelection()
        {
            CanInteract = true;
            OnUpgradePanelRequest?.Invoke(false);
            CameraManager.Instance.RequestCameraChange(CameraIDs.Global.PlayerViewCamera);
        }
    }
}