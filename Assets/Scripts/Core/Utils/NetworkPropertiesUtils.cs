using System;
using Unity.Services.Multiplayer;

namespace OverBang.GameName.Core
{
    public static class NetworkPropertiesUtils
    {
        public static event Action<string, string> OnPropertyChanged;
        
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

        public static void UpdatePlayerProperty(this IPlayer player, string propertyName, PlayerProperty property)
        {
            UpdatePlayerProperty(player, propertyName, property.Value);
        }

        public static void UpdatePlayerProperty(this IPlayer player, string propertyName, string value)
        {
            player.TryGetPlayerProperty(propertyName, out string oldValue);
            
            PlayerProperty property = new PlayerProperty(value);
            player.SetProperty(propertyName, property);
    
            if (oldValue != value)
            {
                OnPropertyChanged?.Invoke(propertyName, value);
            }
        }
        
        public static bool TryGetCharacterDataByPlayer(this IPlayer player, out CharacterData characterData)
        {
            string propertyName = ConstID.Global.PlayerPropertyCharacterData;
            return player.TryGetAssetByPlayerProperty(propertyName, out characterData);
        }

        public static bool TryGetPhaseStatusByPlayer(this IPlayer player, out PhaseStatus phaseStatus)
        {
            string propertyName = ConstID.Global.PlayerPropertyPhaseStatus;
            if (player.TryGetPlayerProperty(propertyName, out string propertyValue))
            {
                if (Enum.TryParse(propertyValue, out phaseStatus))
                {
                    return true;
                }
            }

            phaseStatus = PhaseStatus.None;
            return false;
        }
    }
}