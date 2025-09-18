using Unity.Netcode;

namespace OverBang.GameName.Online.Sessions
{
    public class Client : NetworkBehaviour
    {
        public ulong ClientId { get; private set; }

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                ClientId = OwnerClientId;
                SessionManager.Instance.RegisterSession(this);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                SessionManager.Instance.UnregisterSession(this);
            }
        }
    }
}