using System;
using OverBang.ExoWorld.Core.Interactions;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Cameras;
using UnityEngine;
using UnityUtils;

namespace OverBang.ExoWorld.Gameplay.Upgrade
{
    public class UpgradeTable : MonoBehaviour, IInteractable
    {
        public string InteractionText => string.Empty;
        public int Priority => (int)TargetPriority.High;
        public bool CanInteract { get; private set; } = true;
        public InteractionType SupportedInteractions => InteractionType.Interact;
        Vector3 IInteractable.UIPosition => transform.position.Add(y: 0.5f);
        
        public event Action<bool> OnUpgradePanelRequest;
        
        public void Interact(PlayerInteraction playerInteraction)
        {
            UpgradeManager.Instance.RefreshTable();
            StartUpgradeSelection();
        }

        private void StartUpgradeSelection()
        {
            Debug.Log("Starting upgrade selection");
            Debug.Log(SessionManager.Global.CurrentPlayer);
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