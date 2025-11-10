using Helteix.Tools;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Core;
using OverBang.GameName.Managers;
using OverBang.GameName.Online.Network;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay.Player
{
    public class PlayerController : NetworkBehaviour, IPlayerController
    {
        public NetworkVariable<PlayerNetworkTransform> PlayerState { get; private set; } =
            new NetworkVariable<PlayerNetworkTransform>(writePerm: NetworkVariableWritePermission.Owner);
        
        [SerializeField] private Transform playerModelContainer;
        private IPlayerComponent[] playerComponents;

        private void Awake()
        {
            playerComponents = GetComponentsInChildren<IPlayerComponent>();
        }

        private void Start()
        {
            for (int i = 0; i < playerComponents.Length; i++)
            {
                IPlayerComponent playerComponent = playerComponents[i];
                playerComponent.Controller = this;
            }
        }

        [Rpc(SendTo.Everyone)]
        public void SetDataRpc(string characterDataID)
        {
            if (characterDataID.TryGetAssetByID(out CharacterData characterData))
            {
                for (int i = 0; i < playerComponents.Length; i++)
                {
                    IPlayerComponent  playerComponent = playerComponents[i];
                    playerComponent.OnSync(characterData);
                }
            }
            
            playerModelContainer.ClearChildren();
            Instantiate(characterData.ModelPrefab, playerModelContainer);
        }
    }
}