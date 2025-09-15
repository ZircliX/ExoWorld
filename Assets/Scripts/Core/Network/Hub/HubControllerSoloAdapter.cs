using System;
using KBCore.Refs;
using UnityEngine;

namespace OverBang.GameName.HUB
{
    public class HubControllerSoloAdapter : MonoBehaviour, ICheckForGameStart
    {
        [field: SerializeField, Self] public HubController Hub { get; private set; }
        
        private void OnValidate() => this.ValidateRefs();

        private void OnEnable()
        {
            Hub.OnHubPlayerReadyChanged += OnPlayerReadyChanged;
        }
        
        private void OnDisable()
        {
            Hub.OnHubPlayerReadyChanged -= OnPlayerReadyChanged;
        }

        private void OnPlayerReadyChanged(ulong id, bool newStatus)
        {
            if (newStatus)
            {
                Debug.Log("[Solo] Player is ready, starting game...");
                StartGame();
            }
        }

        public void StartGame()
        {
            Hub.StartGame();
        }
    }
}