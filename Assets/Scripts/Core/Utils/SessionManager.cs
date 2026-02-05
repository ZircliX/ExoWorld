using System;
using System.Collections.Generic;
using OverBang.ExoWorld.Core.Metrics;
using Unity.Netcode;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Utils
{
    public class SessionManager
    {
        public static SessionManager Global => GameController.SessionManager;
        
        public event Action<ISession> OnSessionChanged;
        
        public ISession ActiveSession {get; private set;}

        public IPlayer CurrentPlayer => ActiveSession?.CurrentPlayer;

        public bool IsHost()
        {
            return ActiveSession is { IsHost: true };
        }

        public async Awaitable<ISession> CreateOrJoinSession(string sessionId, SessionOptions options)
        {
            if (ActiveSession != null)
                return ActiveSession;
            
            ActiveSession = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionId, options);
            await SetPlayerName();
            
            OnSessionChanged?.Invoke(ActiveSession);
            return ActiveSession;
        }
        
        public async Awaitable<IHostSession> CreateSession(SessionOptions options)
        {
            if (ActiveSession != null)
                return null;
            
            ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            await SetPlayerName();
            
            OnSessionChanged?.Invoke(ActiveSession);
            return (IHostSession)ActiveSession;
        }
        
        public async Awaitable<ISession> JoinSessionByID(string sessionID)
        {
            if (ActiveSession != null)
                return ActiveSession;
            
            ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID);
            await SetPlayerName();
                
            OnSessionChanged?.Invoke(ActiveSession);
            return ActiveSession;
        }
        
        public async Awaitable<ISession> JoinSessionByCode(string password)
        {
            if (ActiveSession != null)
                return ActiveSession;
            
            ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(password);
            await SetPlayerName();
                
            OnSessionChanged?.Invoke(ActiveSession);
            return ActiveSession;
        }
        
        public async Awaitable<IList<ISessionInfo>> QuerySessions(QuerySessionsOptions options)
        {
            options ??= new QuerySessionsOptions();
            QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(options);
            return results.Sessions;
        }

        public async Awaitable SetPlayerName()
        {
            await ActiveSession.CurrentPlayer.UpdatePlayerProperty(GameMetrics.Global.ConstID.PlayerPropertyPlayerName, PlayerPrefs.GetString(GameMetrics.Global.ConstID.PlayerPropertyPlayerName));
        }
        
        public async Awaitable KickPlayer(string playerID)
        {
            if (!ActiveSession.IsHost) return;
            await ActiveSession.AsHost().RemovePlayerAsync(playerID);
        }

        public async Awaitable LeaveCurrentSession()
        {
            if (ActiveSession != null)
            {
                try
                {
                    await ActiveSession.LeaveAsync();
                    OnSessionChanged?.Invoke(null);
                }
                catch
                {
                    // Game Closed, Ignore
                }
                finally
                {
                    ActiveSession = null;
                }
            }
        }
    }
}