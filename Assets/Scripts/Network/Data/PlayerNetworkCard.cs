using Unity.Netcode;

namespace OverBang.GameName.Network
{
    public struct PlayerNetworkCard : INetworkSerializable
    {
        public byte NetworkId;
        public bool ReadyState;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref NetworkId);
            serializer.SerializeValue(ref ReadyState);
        }
    }
}