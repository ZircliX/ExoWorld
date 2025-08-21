using Unity.Netcode;

namespace OverBang.GameName.Network.Static
{
    public static class NetworkHelpers
    {
        public static bool CanRunNetworkOperation(this NetworkBehaviour behavior)
        {
            return (!behavior.IsSpawned || behavior.NetworkManager == null || !behavior.NetworkManager.IsListening);
        }
    }
}