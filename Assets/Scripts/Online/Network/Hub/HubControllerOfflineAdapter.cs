using System;
using KBCore.Refs;
using OverBang.GameName.Gameplay.Hub;
using UnityEngine;

namespace OverBang.GameName.Online
{
    public class HubControllerOfflineAdapter : MonoBehaviour
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