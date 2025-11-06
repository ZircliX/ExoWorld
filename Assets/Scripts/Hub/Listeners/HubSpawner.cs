using System.Collections.Generic;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Phases;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public class HubSpawner : MonoPhaseListener<HubPhase>
    {
        private Dictionary<IPlayer, GameObject> currentPlayers;
        
        protected override void Begin(HubPhase phase)
        {
            currentPlayers ??= new Dictionary<IPlayer, GameObject>();
            phase.OnCharacterSelected += SpawnPlayer;
        }

        protected override void End(HubPhase phase, bool success)
        {
            phase.OnCharacterSelected -= SpawnPlayer;
        }

        private void SpawnPlayer(IPlayer player, CharacterData characterData)
        {
            foreach ((IPlayer key, GameObject value) in currentPlayers)
            {
                if (key == player)
                {
                    Destroy(value);
                }
            }

            GameObject playerObject = Instantiate(characterData.CharacterPrefab);
            currentPlayers[player] = playerObject;
        }
    }
}