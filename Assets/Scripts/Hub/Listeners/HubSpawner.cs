using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Phases;
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
            NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.
                InstantiateAndSpawn(characterData.CharacterPrefab, destroyWithScene:true, isPlayerObject:true);
        }
    }
}