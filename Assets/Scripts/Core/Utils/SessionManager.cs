using System;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Core
{
    public class SessionManager
    {
        public static SessionManager Global => GameController.SessionManager;
        
        public event Action<ISession> OnSessionChanged; 
        
        public ISession ActiveSession {get; private set;}
        
        public IPlayer CurrentPlayer => ActiveSession.CurrentPlayer;
        public bool IsHost()
        {
            if (ActiveSession == null)
                return false;
            
            return ActiveSession.IsHost;
        }

        public bool IsAllowed => UnityServices.Instance.State != ServicesInitializationState.Initialized
                                 && !AuthenticationService.Instance.IsSignedIn;

        public async Awaitable<ISession> CreateOrJoinSession(string sessionId, SessionOptions options)
        {
            if (ActiveSession != null)
                return ActiveSession;
            
            ActiveSession = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionId, options);
            OnSessionChanged?.Invoke(ActiveSession);
            return ActiveSession;
        }
        
        public async Awaitable<IHostSession> CreateSession(SessionOptions options)
        {
            if (ActiveSession != null)
                return null;
            
            ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            OnSessionChanged?.Invoke(ActiveSession);
            return (IHostSession)ActiveSession;
        }
        
        public async Awaitable<ISession> JoinSessionByID(string sessionID)
        {
            if (ActiveSession != null)
                return ActiveSession;
            
            ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID);
            OnSessionChanged?.Invoke(ActiveSession);
            return ActiveSession;
        }
        
        public async Awaitable<ISession> JoinSessionByCode(string code)
        {
            if (ActiveSession != null)
                return ActiveSession;
            
            ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code);
            OnSessionChanged?.Invoke(ActiveSession);
            return ActiveSession;
        }
        
        public async Awaitable<IList<ISessionInfo>> QuerySessions(QuerySessionsOptions options)
        {
            options ??= new QuerySessionsOptions();
            QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(options);
            return results.Sessions;
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