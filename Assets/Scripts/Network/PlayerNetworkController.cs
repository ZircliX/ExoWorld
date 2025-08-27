using OverBang.GameName.Managers;
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

        public NetworkVariable<bool> IsReady { get; private set; } =
            new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Owner);
        
        public override void OnNetworkSpawn()
        {
            // Server assigns a GUID only once at spawn
            if (IsServer)
            {
                PlayerID.Value = 0x0000;
                Debug.Log($"[Server] Assigned GUID {PlayerID.Value} to player {OwnerClientId}");
            }
            
            for (int i = 0; i < networkChildren.Length; i++)
            {
                networkChildren[i].OnNetworkSpawn(this);
            }
            
            if (PlayerManager.HasInstance && PlayerManager.Instance.IsSpawned)
            {
                RegisterPlayer();
            }
            else
            {
                Debug.Log("PlayerNetworkController waiting for PlayerManager to be ready...");
                PlayerManager.OnInstanceCreated += RegisterPlayer;
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
        
        private void RegisterPlayer()
        {
            PlayerManager.Instance.RegisterPlayer(this);
        }
        
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
            IsReady.Value = readyStatus;
            PlayerManager.Instance.ChangePlayerReadyStatus(PlayerID.Value, readyStatus);
        }
    }
}
