using KBCore.Refs;
using OverBang.GameName.Managers;
using UnityEngine;

namespace OverBang.GameName.Player
{
    public class PlayerControllerSoloAdapter : MonoBehaviour, IPlayerReady
    {
        [field: SerializeField, Self] public PlayerController PlayerController { get; private set; }

        public bool IsReady { get; private set; }

        private void OnValidate() => this.ValidateRefs();

        private void OnEnable()
        {
            PlayerController.OnReadyKeyPressed += ToggleReady;
        }
        
        private void OnDisable()
        {
            PlayerController.OnReadyKeyPressed -= ToggleReady;
        }

        public void ToggleReady()
        {
            IsReady = !IsReady;
            Debug.Log($"[Solo] Player ready state = {IsReady}");
            PlayerManager.Instance?.ChangePlayerReadyStatus(0, IsReady);
        }
        
        private void Start()
        {
            PlayerManager.Instance?.RegisterPlayer(0, PlayerController);
            PlayerController.EnableLocalControls();
        }

        private void OnDestroy()
        {
            PlayerManager.Instance?.UnregisterPlayer(0);
        }
    }
}