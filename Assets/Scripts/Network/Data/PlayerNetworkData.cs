using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Network
{
    public struct PlayerNetworkState : INetworkSerializable
    {
        private Vector3 position;
        private short rotationY;

        internal Vector3 Position
        {
            get => position;
            set => position = value;
        }

        internal Quaternion Rotation
        {
            get => Quaternion.Euler(0, rotationY, 0);
            set => rotationY = (short) value.eulerAngles.y;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer)
            where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotationY);
        }
    }
}