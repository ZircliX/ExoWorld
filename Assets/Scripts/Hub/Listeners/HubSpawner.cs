using OverBang.GameName.Core;
using Unity.Netcode;
using Unity.Services.Multiplayer;

namespace OverBang.GameName.Hub
{
    public class HubSpawner : MonoPhaseListener<HubPhase>
    {
        protected override void OnBegin(HubPhase phase)
        {
            //Debug.Log("OnBegin");
            phase.OnCharacterSelected += SpawnPlayer;
            
            if (phase.SelectedCharacter != null)
                SpawnPlayer(phase.CurrentPlayer, phase.SelectedCharacter);
        }

        protected override void OnEnd(HubPhase phase)
        {
            //Debug.Log("OnEnd");
            phase.OnCharacterSelected -= SpawnPlayer;
        }

        private void SpawnPlayer(IPlayer player, CharacterData characterData)
        {
            //Debug.Log($"Spawn player {player.Id} with character {characterData.AgentName}");
            ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;
            PlayerSpawner.SpawnPlayerObject(characterData, clientID, SessionManager.Global.CurrentPlayer);
        }
    }
}