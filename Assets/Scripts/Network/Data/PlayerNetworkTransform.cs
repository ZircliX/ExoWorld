using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Network
{
    public struct PlayerNetworkTransform : INetworkSerializable
    {
        private Vector3 position;
        private float rotationY;

        public Vector3 Position
        {
            get => position;
            internal set => position = value;
        }

        public Quaternion Rotation
        {
            get => Quaternion.Euler(0, rotationY, 0);
            internal set => rotationY = value.eulerAngles.y;
        }

        public float RotationY => rotationY;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer)
            where T : IReaderWriter
        {
            serializer.SerializeValue(ref position);
            serializer.SerializeValue(ref rotationY);
        }
    }
}