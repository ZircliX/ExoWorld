using UnityEngine;

namespace OverBang.GameName.Core.Database
{
    [CreateAssetMenu(fileName = "New GameDatabase", menuName = "OverBang/GameDatabase", order = 0)]
    public class GameDatabase : ScriptableObject
    {
        public static GameDatabase Global => GameController.GameDatabase;
        
        [field: SerializeField] public ScriptableObject[] DatabaseAssets { get; private set; }

        public bool TryGetAssetByID<T>(string id, out T asset) where T : IDatabaseAsset
        {
            asset = default;
            
            for (int i = 0; i < DatabaseAssets.Length; i++)
            {
                if (DatabaseAssets[i] is IDatabaseAsset dbAsset && dbAsset.ID == id)
                {
                    asset = (T)dbAsset;
                    return true;
                }
            }

            return false;
        }
    }
}