using Helteix.Tools;
using OverBang.GameName.Core;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.GameName.Gameplay
{
    public class PlayerController : NetworkBehaviour, IPlayerController
    {
        [SerializeField] private Transform playerModelContainer;
        private IPlayerComponent[] playerComponents;
        
        private void Awake()
        {
            playerComponents = GetComponentsInChildren<IPlayerComponent>();
        }
        
        [Rpc(SendTo.Everyone)]
        public void SetDataRpc(string characterDataID)
        {
            if (characterDataID.TryGetAssetByID(out CharacterData characterData))
            {
                playerModelContainer.ClearChildren();
                GameObject playerModel = Instantiate(characterData.ModelPrefab, playerModelContainer);
                
                if (!playerModel.TryGetComponent(out Animator playerAnimator))
                {
                    Debug.LogError("Player Model does not have an Animator !");
                }

                if (!playerModel.TryGetComponent(out PlayerRig playerRig))
                {
                    Debug.LogError("Player Model does not have a PlayerRig !");
                }
                
                for (int i = 0; i < playerComponents.Length; i++)
                {
                    IPlayerComponent  playerComponent = playerComponents[i];
                    PlayerRuntimeContext context = new PlayerRuntimeContext()
                    {
                        playerController = this,
                        playerCharacterData = characterData,
                        playerAnimator = playerAnimator,
                        PlayerRig = playerRig,
                    };
                    playerComponent.OnSync(context);
                }
                
                PlayerManager.Instance.RegisterPlayer(this);

                if (IsOwner)
                {
                    CameraManager.Instance.RequestCameraChange(CameraIDs.Global.PlayerViewCamera);
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