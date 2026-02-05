using System;
using System.Threading;
using OverBang.ExoWorld.Core.Database;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Phases;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Utils
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

        public static async Awaitable UpdatePlayerProperty(this IPlayer player, string propertyName, PlayerProperty property)
        {
            await UpdatePlayerProperty(player, propertyName, property.Value);
        }

        public static async Awaitable UpdatePlayerProperty(this IPlayer player, string propertyName, string value)
        {
            player.TryGetPlayerProperty(propertyName, out string oldValue);
            
            PlayerProperty property = new PlayerProperty(value);
            player.SetProperty(propertyName, property);
            await SessionManager.Global.ActiveSession.SaveCurrentPlayerDataAsync();
    
            if (oldValue != value)
            {
                OnPropertyChanged?.Invoke(propertyName, value);
            }
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
        
        // IReadOnlyPlayer version
        
        public static bool TryGetPlayerProperty(this IReadOnlyPlayer player, string propertyName, out string propertyValue)
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
        
        public static bool TryGetPhaseStatusByPlayer(this IReadOnlyPlayer player, out PhaseStatus phaseStatus)
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
        
        // Await until players properties reaches a specific value

        public static async Awaitable AwaitableUntilAllPlayers(PhaseStatus targetStatus)
        {
            await AwaitableUtils.AwaitableUntil(() =>
            {
                
                if (SessionManager.Global.ActiveSession == null) return false;
                bool allReady = true;
                
                foreach (IReadOnlyPlayer player in SessionManager.Global.ActiveSession.Players)
                {
                    if (player.TryGetPhaseStatusByPlayer(out PhaseStatus status))
                    {
                        Debug.Log(status.ToString());
                        if (status != targetStatus)
                            allReady = false;
                    }
                }
                Debug.Log(allReady);
                return allReady;
            }, CancellationToken.None);
        } 
    }
}