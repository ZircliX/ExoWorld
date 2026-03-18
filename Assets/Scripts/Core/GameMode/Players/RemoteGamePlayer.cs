using System;
using System.Linq;
using OverBang.ExoWorld.Core.Characters;
using OverBang.ExoWorld.Core.Metrics;
using OverBang.ExoWorld.Core.Utils;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.ExoWorld.Core.GameMode.Players
{
    public class RemoteGamePlayer : IGamePlayer
    {

        public float Health
        {
            get
            {
                if (!SessionPlayer.TryGetPlayerProperty(ConstID.Global.PlayerPropertyHealth, out string propertyValue))
                    return 0;
                return float.Parse(propertyValue);
            }
        }
        
        public float MaxHealth
        {
            get
            {
                if (!SessionPlayer.TryGetPlayerProperty(ConstID.Global.PlayerPropertyMaxHealth,
                        out string propertyValue)) return 0;
                return float.Parse(propertyValue);
            }
        }


        public ulong ClientID
        {
            get
            {
                if (!SessionPlayer.TryGetPlayerProperty(ConstID.Global.PlayerPropertyClientID,
                        out string propertyValue)) return 0;
                return ulong.Parse(propertyValue);
            }
        }

        public PlayerState State 
        {
            get
            {
                if (!SessionPlayer.TryGetPlayerProperty(ConstID.Global.PlayerPropertyState, out string propertyValue))
                    return PlayerState.Uninitialized;
                return Enum.Parse<PlayerState>(propertyValue);
            }
        }

        public CharacterData CharacterData
        {
            get
            {
                if (!SessionPlayer.TryGetPlayerProperty(ConstID.Global.PlayerPropertyState, out string propertyValue))
                    return null;
                
                return propertyValue.TryGetAssetByID(out CharacterData data) ? data : null;
            }
        }

        public IReadOnlyPlayer SessionPlayer => SessionManager.Global.ActiveSession.
            Players.FirstOrDefault(ctx => ctx.Id == SessionPlayerID);
        
        public string SessionPlayerID { get; }

        public RemoteGamePlayer(string id)
        {
            SessionPlayerID = id;
        }
    }
}
