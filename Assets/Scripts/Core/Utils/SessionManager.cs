using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Core
{
    public class SessionManager
    {
        public static SessionManager Global => GameController.SessionManager;
        
        public ISession ActiveSession {get; private set;}
        
        public IPlayer CurrentPlayer => ActiveSession.CurrentPlayer;
        public bool IsHost()
        {
            if (ActiveSession == null)
                return false;
            
            return ActiveSession.IsHost;
        }

        public async Awaitable<ISession> CreateOrJoinSession(string sessionId, SessionOptions options)
        {
            ActiveSession = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionId, options);
            return ActiveSession;
        }
        
        public async Awaitable<IHostSession> CreateSession(SessionOptions options)
        {
            ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            return (IHostSession)ActiveSession;
        }
        
        public async Awaitable<ISession> JoinSessionByID(string sessionID)
        {
            ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID);
            return ActiveSession;
        }
        
        public async Awaitable<ISession> JoinSessionByCode(string code)
        {
            ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code);
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