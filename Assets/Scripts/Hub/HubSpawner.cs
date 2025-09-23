using OverBang.GameName.Core.Characters;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OverBang.GameName.CharacterSelection
{
    public class HubSpawner : HubListener
    {
        private GameObject currentPlayer;
        
        protected internal override void OnInit(HubPhase phase)
        {
            phase.OnCharacterSelected += SpawnPlayer;
        }

        protected internal override void OnRelease(HubPhase phase)
        {
            phase.OnCharacterSelected -= SpawnPlayer;
        }
        
        private void SpawnPlayer(CharacterData characterData)
        {
            //Debug.Log($"Spawning player with character: {characterData.AgentName}");

            if (!characterData.CharacterPrefabRef.IsValid() || !characterData.CharacterPrefabRef.IsDone)
            {
                AsyncOperationHandle<GameObject> handle = characterData.CharacterPrefabRef.LoadAssetAsync<GameObject>();
                
                handle.Completed += op =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        SpawnPlayer(op.Result);
                    }
                    else
                    {
                        Debug.LogError("Failed to load player prefab.");
                    }
                };
            }
            else
            {
                SpawnPlayer(characterData.CharacterPrefabRef.Asset as GameObject);
            }
        }
        
        private void SpawnPlayer(GameObject player)
        {
            if (currentPlayer != null)
            {
                Destroy(currentPlayer);
            }

            currentPlayer = Instantiate(player);
        }
    }
}