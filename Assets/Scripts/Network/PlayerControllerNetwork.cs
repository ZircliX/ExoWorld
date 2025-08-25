using OverBang.GameName.Managers;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Network
{
    public class PlayerNetworkController : NetworkBehaviour
    {
        [SerializeReference] private NetworkChildren[] networkChildren;
        
        public NetworkVariable<PlayerNetworkTransform> PlayerState { get; private set; } = 
            new NetworkVariable<PlayerNetworkTransform>(writePerm: NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<FixedString64Bytes> PlayerGuid { get; private set; } = 
            new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Server);

        public NetworkVariable<bool> IsReady { get; private set; } =
            new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);
        
        public override void OnNetworkSpawn()
        {
            // Server assigns a GUID only once at spawn
            if (IsServer)
            {
                PlayerGuid.Value = new FixedString64Bytes(System.Guid.NewGuid().ToString());
                Debug.Log($"[Server] Assigned GUID {PlayerGuid.Value} to player {OwnerClientId}");
            }
            
            for (int i = 0; i < networkChildren.Length; i++)
            {
                networkChildren[i].OnNetworkSpawn(this);
            }
        }
        
        public override void OnNetworkDespawn()
        {
            for (int i = 0; i < networkChildren.Length; i++)
            {
                networkChildren[i].OnNetworkDespawn();
            }
        }
        
        private void Update()
        {
            for (int i = 0; i < networkChildren.Length; i++)
            {
                networkChildren[i].OnUpdate();
            }
        }
        
        // --- Public Methods ---
        
        public void WritePlayerReadyStatus(PlayerNetworkTransform playerTransform)
        {
            PlayerState.Value = playerTransform;
        }

        public void WritePlayerReadyStatus(bool readyStatus)
        {
            IsReady.Value = readyStatus;
            PlayerManager.Instance.ChangePlayerReadyStatus(PlayerGuid.Value.ToString(), readyStatus);
        }
    }
}
