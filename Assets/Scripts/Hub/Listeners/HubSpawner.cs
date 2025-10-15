using System;
using System.Collections.Generic;
using OverBang.GameName.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OverBang.GameName.Hub
{
    public class HubSpawner : HubListener
    {
        private Dictionary<int, GameObject> currentPlayers;
        
        protected internal override void OnInit(HubPhase phase)
        {
            currentPlayers ??= new Dictionary<int, GameObject>(phase.Settings.PlayerCount);
            phase.OnCharacterSelected += SpawnPlayer;
        }

        protected internal override void OnRelease(HubPhase phase)
        {
            phase.OnCharacterSelected -= SpawnPlayer;
            ReleaseTrackedResources();
        }

        private void SpawnPlayer(int id, PlayerProfile playerProfile)
        {
            if (currentPlayers.Remove(id, out GameObject toRelease))
            {
                Addressables.ReleaseInstance(toRelease);
            }

            Addressables.InstantiateAsync(playerProfile.characterData.CharacterPrefabRef).Completed +=
                handle =>
                {
                    if (handle.Status == AsyncOperationStatus.Succeeded)
                    {
                        currentPlayers[id] = handle.Result;
                    }
                };
        }

        public void ReleaseTrackedResources()
        {
            if (currentPlayers == null) return;

            foreach ((int key, GameObject o) in currentPlayers)
            {
                Addressables.ReleaseInstance(o);
            }

            currentPlayers = null;
        }
    }
}