using OverBang.GameName.Core.Characters;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace OverBang.GameName.Core
{
    public interface IPlayerSpawner
    {
        void SpawnPlayer(CharacterData characterData);

        void OnInstantiateCompleteObjectTracked(AsyncOperationHandle<GameObject> handle);

        void ReleaseTrackedResources();
    }
}