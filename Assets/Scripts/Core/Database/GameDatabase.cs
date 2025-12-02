using UnityEngine;
using InterfaceAttributes;

namespace OverBang.GameName.Core
{
    [CreateAssetMenu(fileName = "New GameDatabase", menuName = "OverBang/GameDatabase", order = 0)]
    public class GameDatabase : ScriptableObject
    {
        public static GameDatabase Global => GameController.GameDatabase;
        
        [field: SerializeField]
        public InterfaceReference<IDatabaseAsset, ScriptableObject>[] DatabaseAssets { get; private set; }

        public bool TryGetAssetByID<T>(string id, out T asset) where T : IDatabaseAsset
        {
            asset = default;
            
            for (int i = 0; i < DatabaseAssets.Length; i++)
            {
                InterfaceReference<IDatabaseAsset, ScriptableObject> interfaceReference = DatabaseAssets[i];
                if (interfaceReference.Value.ID == id)
                {
                    asset = (T)interfaceReference;
                    return true;
                }
            }

            return false;
        }
    }
}