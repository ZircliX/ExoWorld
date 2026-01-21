using Unity.Netcode;
using UnityEngine;
using Object = UnityEngine.Object;

namespace OverBang.ExoWorld.Core
{
    public static class PlayerSpawner
    {
        public static NetworkObject SpawnPlayerObject(CharacterData characterData, ulong clientId, Vector3 position, Quaternion rotation)
        {
            NetworkObject playerObject = Object.Instantiate(GameMetrics.Global.PlayerControllerPrefab, position, rotation);
            playerObject.SpawnAsPlayerObject(clientId, destroyWithScene: true);
            //Debug.Log($"Instantiated player object {playerObject.name}", playerObject);

            if (playerObject.TryGetComponent(out IPlayerController playerController))
            {
                playerController.SetDataRpc(characterData.ID);
            }
            
            return playerObject;
        }
    }
}