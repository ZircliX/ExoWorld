using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.GameName.Core
{
    public static class PlayerSpawner
    {
        public static NetworkObject SpawnPlayerObject(CharacterData characterData, ulong clientId, IPlayer player)
        {
            NetworkObject playerObject = Object.Instantiate(GameMetrics.Global.PlayerControllerPrefab);
            playerObject.SpawnAsPlayerObject(clientId, destroyWithScene: true);
            Debug.Log($"Instantiated player object {playerObject.name}", playerObject);

            if (playerObject.TryGetComponent(out IPlayerController playerController))
            {
                playerController.SetDataRpc(characterData.ID);
            }
            
            player.UpdatePlayerProperty(ConstID.Global.PlayerPropertyPhaseStatus, nameof(PhaseStatus.PlayerSetup));

            return playerObject;
        }
    }
}