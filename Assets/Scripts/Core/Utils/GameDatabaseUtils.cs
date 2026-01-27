using OverBang.ExoWorld.Core.Database;

namespace OverBang.ExoWorld.Core.Utils
{
    public static class GameDatabaseUtils
    {
        public static bool TryGetAssetByID<T>(this string id, out T asset)  where T : IDatabaseAsset
        {
            return GameDatabase.Global.TryGetAssetByID(id, out asset);
        }
    }
}