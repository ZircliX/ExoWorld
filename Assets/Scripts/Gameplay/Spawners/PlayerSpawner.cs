using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Gameplay.Gameplay.Listeners;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OverBang.GameName.Gameplay.Gameplay.Spawners
{
    public class GameplaySpawner : GameplayListener, IPlayerSpawner
    {
        GameObject currentPlayer;
        
        protected internal override void OnInit(GameplayPhase phase)
        {
            phase.OnSpawnPlayer += SpawnPlayer;
        }

        protected internal override void OnRelease(GameplayPhase phase)
        {
            phase.OnSpawnPlayer -= SpawnPlayer;
            ReleaseTrackedResources();
        }

        public void SpawnPlayer(CharacterData characterData)
        {
            Debug.Log( $"Spawning player with character data: {characterData.AgentName}");
            if (currentPlayer != null)
            {
                ReleaseTrackedResources();
            }
            
            Addressables.InstantiateAsync(characterData.CharacterPrefabRef).Completed += OnInstantiateCompleteObjectTracked;
        }

        public void OnInstantiateCompleteObjectTracked(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                currentPlayer = handle.Result;
                Debug.Log($"Successfully instantiated GameObject with name ''.");
            }
        }
        
        public void ReleaseTrackedResources()
        {
            if (currentPlayer == null) return;
            
            Addressables.ReleaseInstance(currentPlayer);
            currentPlayer = null;
        }
    }
}