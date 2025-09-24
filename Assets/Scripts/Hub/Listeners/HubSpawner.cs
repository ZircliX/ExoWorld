using OverBang.GameName.Core;
using OverBang.GameName.Core.Characters;
using OverBang.GameName.Core.Pooling;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OverBang.GameName.Hub
{
    public class HubSpawner : HubListener, IPlayerSpawner
    {
        GameObject currentPlayer;
        
        protected internal override void OnInit(HubPhase phase)
        {
            phase.OnCharacterSelected += SpawnPlayer;
        }

        protected internal override void OnRelease(HubPhase phase)
        {
            phase.OnCharacterSelected -= SpawnPlayer;
            ReleaseTrackedResources();
        }
        
        public void SpawnPlayer(CharacterData characterData)
        {
            if (currentPlayer != null)
            {
                ReleaseTrackedResources();
            }

            PoolableObject test = GameController.PoolManager.Spawn<PoolableObject>("Player Pool");
            
            //Addressables.InstantiateAsync(characterData.CharacterPrefabRef).Completed += OnInstantiateCompleteObjectTracked;
        }

        public void OnInstantiateCompleteObjectTracked(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                currentPlayer = handle.Result;
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