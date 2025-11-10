using Unity.Services.Multiplayer;

namespace OverBang.GameName.Core
{
    public static class NetworkPropertiesUtils
    {
        public static bool TryGetPlayerProperty(this IPlayer player, string propertyName, out string propertyValue)
        {
            if (player?.Properties != null &&
                player.Properties.TryGetValue(propertyName, out PlayerProperty prop) &&
                !string.IsNullOrEmpty(prop.Value))
            {
                propertyValue = prop.Value;
                return true;
            }

            propertyValue = string.Empty;
            return false;
        }

        public static bool TryGetAssetByPlayerProperty<T>(this IPlayer player, string propertyName, out T asset)
            where T : IDatabaseAsset
        {
            asset = default(T);

            return player.TryGetPlayerProperty(propertyName, out string propertyValue) 
                   && propertyValue.TryGetAssetByID(out asset);
        }

        public static bool TryGetCharacterDataByPlayer(this IPlayer player, out CharacterData characterData)
        {
            string characterDataPropertyName = ConstID.Global.PlayerPropertyCharacterData;
            return player.TryGetAssetByPlayerProperty(characterDataPropertyName, out characterData);
        }
    }
}