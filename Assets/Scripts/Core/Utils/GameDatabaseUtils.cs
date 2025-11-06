using OverBang.GameName.Core.Database;

namespace OverBang.GameName.Managers
{
    public static class GameDatabaseUtils
    {
        public static bool TryGetAssetByID<T>(this string id, out T asset)  where T : IDatabaseAsset
        {
            return GameDatabase.Global.TryGetAssetByID(id, out asset);
        }
    }
}