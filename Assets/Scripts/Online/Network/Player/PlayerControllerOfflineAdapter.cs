using KBCore.Refs;
using OverBang.GameName.Gameplay.Player;
using UnityEngine;

namespace OverBang.GameName.Online.Network
{
    public class PlayerControllerOfflineAdapter : MonoBehaviour
    {
        [field: SerializeField, Self] public PlayerController PlayerController { get; private set; }

        public bool IsReady { get; private set; }

        private void OnValidate() => this.ValidateRefs();

        public void ToggleReady()
        {
            IsReady = !IsReady;
            Debug.Log($"[Solo] Player ready state = {IsReady}");
            PlayerManager.Instance?.ChangePlayerReadyStatus(0, IsReady);
        }
        
        private void OnEnable()
        {
            PlayerManager.Instance?.RegisterPlayer(0, PlayerController);
            PlayerController.EnableLocalControls();
        }

        private void OnDisable()
        {
            PlayerManager.Instance?.UnregisterPlayer(0);
        }
    }
}