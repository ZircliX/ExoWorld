using Helteix.Tools;
using OverBang.GameName.Core;
using OverBang.GameName.Gameplay.Data;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PlayerController : NetworkBehaviour, IPlayerController
    {
        public NetworkVariable<PlayerNetworkTransform> PlayerState { get; private set; } =
            new NetworkVariable<PlayerNetworkTransform>(writePerm: NetworkVariableWritePermission.Owner);
        
        [field : SerializeField] public Transform PlayerTransform {get ; private set;}
        [SerializeField] private Transform playerModelContainer;
        private IPlayerComponent[] playerComponents;
        
        private Animator playerAnimator;

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

            PlayerManager.Instance.RegisterPlayer(this);
        }

        [Rpc(SendTo.Everyone)]
        public void SetDataRpc(string characterDataID)
        {
            if (characterDataID.TryGetAssetByID(out CharacterData characterData))
            {
                playerModelContainer.ClearChildren();
                GameObject playerModel = Instantiate(characterData.ModelPrefab, playerModelContainer);
                if (!playerModel.TryGetComponent(out playerAnimator))
                {
                    Debug.LogError("Player Model does not have an Animator !");
                }
                
                for (int i = 0; i < playerComponents.Length; i++)
                {
                    IPlayerComponent  playerComponent = playerComponents[i];
                    playerComponent.OnSync(characterData, playerAnimator);
                }
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            PlayerManager.Instance.UnregisterPlayer(this);
            
        }
    }
}