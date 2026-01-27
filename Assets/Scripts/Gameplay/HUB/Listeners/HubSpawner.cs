using OverBang.ExoWorld.Core.GameMode.Players;
using OverBang.ExoWorld.Core.Phases;
using OverBang.ExoWorld.Core.Utils;
using OverBang.ExoWorld.Gameplay.Phase;
using Unity.Netcode;
using UnityEngine;

namespace OverBang.ExoWorld.Gameplay.HUB.Listeners
{
    public class HubSpawner : MonoPhaseListener<HubPhase>
    {
        protected override void OnBegin(HubPhase phase)
        {
            //Debug.Log("OnBegin");
            phase.OnCharacterSelected += SpawnPlayer;

            //return; // No spawn at first hub phase
            if (phase.SelectedCharacter != null)
                SpawnPlayer(GamePlayerManager.Instance.GetLocalPlayer(), false);
        }

        protected override void OnEnd(HubPhase phase)
        {
            //Debug.Log("OnEnd");
            phase.OnCharacterSelected -= SpawnPlayer;
        }

        private void SpawnPlayer(LocalGamePlayer player, bool characterChanged)
        {
            //Debug.Log($"Spawn player {player.Id} with character {characterData.AgentName}");
            ulong clientID = NetworkManager.Singleton.LocalClient.ClientId;
            
            //TODO : FOUDROYER LOIS
            Vector3 position = new Vector3(0f, 0f, 0);
            Quaternion rotation = Quaternion.Euler(0f, 0, 0f);
            player.Spawn(position, rotation);
            
            Awaitable awaitable = PoolUtils.SetupPooling();
            awaitable.Run();
        }
    }
}