using UnityEngine;

namespace OverBang.GameName.Network
{
    [System.Serializable]
    public struct PlayerNetworkData
    {
        [field: SerializeField] public bool ReadyStatus { get; internal set; }
        [field: SerializeField] public float Rotation { get; internal set; }
        [field: SerializeField] public Vector3 Position { get; internal set; }
        [field: SerializeField] public string ClientId { get; internal set; }
        [field: SerializeField] public string Authority { get; internal set; }
        [field: SerializeField] public string Ping { get; internal set; }
    }
}