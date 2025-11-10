using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Core;
using OverBang.GameName.Core.Metrics;
using OverBang.GameName.Core.Phases;
using OverBang.GameName.Managers;
using Unity.Netcode;
using Unity.Services.Multiplayer;

namespace OverBang.GameName.Hub
{
    public class HubSpawner : MonoPhaseListener<HubPhase>
    {
        protected override void Begin(HubPhase phase)
        {
            phase.OnCharacterSelected += SpawnPlayer;
        }

        protected override void End(HubPhase phase, bool success)
        {
            phase.OnCharacterSelected -= SpawnPlayer;
        }

        private void SpawnPlayer(IPlayer player, CharacterData characterData)
        {
            ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;
            NetworkObject playerObject = Instantiate(GameMetrics.Global.PlayerControllerPrefab);
            playerObject.SpawnAsPlayerObject(clientID, destroyWithScene: true);

            if (playerObject.TryGetComponent(out IPlayerController playerController))
            {
                playerController.SetDataRpc(characterData.ID);
            }
        }
    }
}