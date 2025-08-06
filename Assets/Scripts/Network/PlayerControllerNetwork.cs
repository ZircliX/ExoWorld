using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Network
{
    public class PlayerNetworkController : NetworkBehaviour
    {
        [SerializeReference] private NetworkChildren[] networkChildren;
        
        public NetworkVariable<PlayerNetworkState> PlayerState { get; private set; } = 
            new NetworkVariable<PlayerNetworkState>(writePerm: NetworkVariableWritePermission.Owner);
        
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
        
        public void WritePlayerState(PlayerNetworkState state)
        {
            PlayerState.Value = state;
        }
    }
}
