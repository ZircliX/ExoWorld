using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Network
{
    public class PlayerNetworkController : NetworkBehaviour
    {
        [SerializeReference] private NetworkChildren[] networkChildren;
        
        public NetworkVariable<PlayerNetworkTransform> PlayerState { get; private set; } = 
            new NetworkVariable<PlayerNetworkTransform>(writePerm: NetworkVariableWritePermission.Owner);
        
        public NetworkVariable<byte> PlayerID { get; private set; } = 
            new NetworkVariable<byte>(writePerm: NetworkVariableWritePermission.Server);

        public bool IsReady { get; private set; }        
        public override void OnNetworkSpawn()
        {
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
        
        // --- Private Methods ---
        
        // --- Public Methods ---
        
        public void WritePlayerNetworkTransform(PlayerNetworkTransform playerTransform)
        {
            PlayerState.Value = playerTransform;
        }

        public void WritePlayerID(byte playerID)
        {
            PlayerID.Value = playerID;
        }
        
        public void WritePlayerReadyStatus(bool readyStatus)
        {
            Debug.LogError($"Player {PlayerID.Value} has ready status: {readyStatus}");
            IsReady = readyStatus;
        }
    }
}
