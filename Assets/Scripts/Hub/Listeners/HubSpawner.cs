using System.Collections.Generic;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Phases;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public class HubSpawner : MonoPhaseListener<HubPhase>
    {
        private Dictionary<IPlayer, NetworkObject> currentPlayers;
        
        protected override void Begin(HubPhase phase)
        {
            currentPlayers ??= new Dictionary<IPlayer, NetworkObject>();
            phase.OnCharacterSelected += SpawnPlayer;
        }

        protected override void End(HubPhase phase, bool success)
        {
            phase.OnCharacterSelected -= SpawnPlayer;
            foreach (KeyValuePair<IPlayer, NetworkObject> player in currentPlayers)
            {
                Destroy(player.Value.gameObject);
            }
        }

        private void SpawnPlayer(IPlayer player, CharacterData characterData)
        {
            foreach ((IPlayer key, NetworkObject value) in currentPlayers)
            {
                if (key == player)
                {
                    Destroy(value);
                }
            }

            NetworkObject playerObject = NetworkManager.Singleton.SpawnManager.InstantiateAndSpawn(characterData.CharacterPrefab);
            currentPlayers[player] = playerObject;
        }
    }
}