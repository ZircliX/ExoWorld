using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Network
{
    [DefaultExecutionOrder(10)]
    public class PlayerNetworkController : NetworkBehaviour
    {
        [SerializeReference] private NetworkChildren[] networkChildren;
        
        public NetworkVariable<PlayerNetworkState> PlayerState { get; private set; } = 
            new NetworkVariable<PlayerNetworkState>(writePerm: NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<FixedString64Bytes> PlayerGuid { get; private set; } = 
            new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Server);
        
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
        
        public void WritePlayerState(PlayerNetworkState state)
        {
            PlayerState.Value = state;
        }
    }
}
