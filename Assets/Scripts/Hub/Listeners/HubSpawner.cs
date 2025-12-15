using OverBang.GameName.Core;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public class HubSpawner : MonoPhaseListener<HubPhase>
    {
        protected override void OnBegin(HubPhase phase)
        {
            //Debug.Log("OnBegin");
            phase.OnCharacterSelected += SpawnPlayer;
            
            if (phase.SelectedCharacter != null)
                SpawnPlayer(phase.CurrentPlayer, phase.SelectedCharacter, false);
        }

        protected override void OnEnd(HubPhase phase)
        {
            //Debug.Log("OnEnd");
            phase.OnCharacterSelected -= SpawnPlayer;
        }

        private void SpawnPlayer(IPlayer player, CharacterData characterData, bool characterChanged)
        {
            //Debug.Log($"Spawn player {player.Id} with character {characterData.AgentName}");
            ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;
            Vector3 position = new Vector3(0f, 0f, 0);
            Quaternion rotation = Quaternion.Euler(0f, 0, 0f);
            PlayerSpawner.SpawnPlayerObject(characterData, clientID, position, rotation);
            
            Awaitable awaitable = PoolUtils.SetupPooling();
            awaitable.Run();
        }
    }
}