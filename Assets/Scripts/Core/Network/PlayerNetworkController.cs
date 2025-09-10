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

        public NetworkVariable<ulong> PlayerID { get; private set; } =
            new NetworkVariable<ulong>(writePerm: NetworkVariableWritePermission.Server);

        public NetworkVariable<bool> IsReady { get; private set; } =
            new NetworkVariable<bool>(writePerm: NetworkVariableWritePermission.Server);

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

        // --- Public Methods ---

        public void WritePlayerID(ulong playerID)
        {
            PlayerID.Value = playerID;
        }

        private void WritePlayerReadyStatus(bool readyStatus)
        {
            IsReady.Value = readyStatus;
        }

        [Rpc(SendTo.Server)]
        public void RequestSetReadyRpc(bool newReadyStatus)
        {
            WritePlayerReadyStatus(newReadyStatus);
            PlayerManager.Instance.ChangePlayerReadyStatus(PlayerID.Value, newReadyStatus);
        }
    }
}