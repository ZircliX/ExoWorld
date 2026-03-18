using OverBang.ExoWorld.Core.Characters;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Audios.ContextualDialogues
{
    public struct CDContext : INetworkSerializable
    {
        public string characterDataId;
        public ulong playerId;

        public NetworkObjectReference networkObject;
        public Vector3 sourcePosition;
        public SourceType sourceType;

        public enum SourceType
        {
            FollowSpatialized,
            FixedSpatialized,
            Static
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref characterDataId);
            serializer.SerializeValue(ref playerId);
            serializer.SerializeValue(ref networkObject);
            serializer.SerializeValue(ref sourcePosition);
            serializer.SerializeValue(ref sourceType);
        }
    }
}