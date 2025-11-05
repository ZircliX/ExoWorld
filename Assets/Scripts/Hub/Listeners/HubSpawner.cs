using System.Collections.Generic;
using OverBang.GameName.Core;
using UnityEngine;

namespace OverBang.GameName.Hub
{
    public class HubSpawner : HubListener
    {
        private Dictionary<PlayerProfile, GameObject> currentPlayers;
        
        protected override void Begin(HubPhase phase)
        {
            currentPlayers ??= new Dictionary<PlayerProfile, GameObject>();
            phase.OnCharacterSelected += SpawnPlayer;
        }

        protected override void End(HubPhase phase, bool success)
        {
            phase.OnCharacterSelected -= SpawnPlayer;
        }

        private void SpawnPlayer(PlayerProfile playerProfile)
        {
            foreach ((PlayerProfile key, GameObject value) in currentPlayers)
            {
                if (key.characterData.ID == playerProfile.characterData.ID)
                {
                    Destroy(value);
                }
            }

            GameObject player = Instantiate(playerProfile.characterData.CharacterPrefab);
            currentPlayers[playerProfile] = player;
        }
    }
}