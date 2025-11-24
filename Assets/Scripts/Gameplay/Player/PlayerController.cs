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
        
        [SerializeField] private Transform playerModelContainer;
        [field : SerializeField] public Transform playerTransform {get ; private set;}
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

            PlayerManager.Instance.RegisterPlayer(this);
        }

        [Rpc(SendTo.Everyone)]
        public void SetDataRpc(string characterDataID)
        {
            if (characterDataID.TryGetAssetByID(out CharacterData characterData))
            {
                for (int i = 0; i < playerComponents.Length; i++)
                {
                    IPlayerComponent  playerComponent = playerComponents[i];
                    
                    playerModelContainer.ClearChildren();
                    GameObject playerModel = Instantiate(characterData.ModelPrefab, playerModelContainer);
                    Animator playerAnimator = playerModel.GetComponent<Animator>();
                    
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