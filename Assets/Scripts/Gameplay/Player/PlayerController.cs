using Helteix.Tools;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Database;
using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Gameplay.Cameras;
using OverBang.ExoWorld.Gameplay.IK_Animation;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.Player
{
    public class PlayerController : NetworkBehaviour, IPlayerController
    {
        [SerializeField] private Transform playerModelContainer;
        private IPlayerComponent[] playerComponents;
        public LocalGamePlayer LocalGamePlayer { get; private set; }
        
        private void Awake()
        {
            playerComponents = GetComponentsInChildren<IPlayerComponent>();
        }
        
        public void Connect(LocalGamePlayer localGamePlayer)
        {
            LocalGamePlayer = localGamePlayer;
            SetDataRpc(localGamePlayer.SessionPlayerID, localGamePlayer.CharacterData.ID);
        }
        
        [Rpc(SendTo.Everyone)]
        public void SetDataRpc(string sessionPlayer, string characterDataId)
        {
            if (GamePlayerManager.Instance.TryGetPlayerWithSessionId(sessionPlayer, out IGamePlayer player) &&
                GameDatabase.Global.TryGetAssetByID(characterDataId, out CharacterData data))
            {
                playerModelContainer.ClearChildren();
                GameObject playerModel = Instantiate(data.ModelPrefab, playerModelContainer);
                
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
                        playerCharacterData = data,
                        playerAnimator = playerAnimator,
                        PlayerRig = playerRig,
                    };
                    playerComponent.OnSync(context);
                }
                
                if (IsOwner)
                {
                    CameraManager.Instance.RequestCameraChange(CameraIDs.Global.PlayerViewCamera);
                }
            }
        }
    }
}