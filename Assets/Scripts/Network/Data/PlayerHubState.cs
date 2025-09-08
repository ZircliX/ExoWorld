using System;
using Unity.Netcode;

namespace OverBang.GameName.Network
{
    public struct PlayerHubState : IEquatable<PlayerHubState>, INetworkSerializable
    {
        public byte PlayerID;
        public bool IsReady;

        public PlayerHubState(byte playerId, bool isReady)
        {
            PlayerID = playerId;
            IsReady = isReady;
        }

        public void SetReady(bool isReady)
        {
            IsReady = isReady;
        }

        public bool Equals(PlayerHubState other)
        {
            return PlayerID == other.PlayerID && IsReady == other.IsReady;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerHubState other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(PlayerID, IsReady);
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref PlayerID);
            serializer.SerializeValue(ref IsReady);
        }
    }
}