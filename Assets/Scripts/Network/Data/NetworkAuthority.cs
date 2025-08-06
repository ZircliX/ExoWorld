namespace OverBang.GameName.Network
{
    public struct NetworkAuthority
    {
        public readonly bool IsOwner;
        public readonly bool IsServer;
        public readonly bool IsClient;
        public readonly bool IsHost;
        
        public NetworkAuthority(bool isOwner, bool isServer, bool isClient, bool isHost)
        {
            IsOwner = isOwner;
            IsServer = isServer;
            IsClient = isClient;
            IsHost = isHost;
        }
    }
}