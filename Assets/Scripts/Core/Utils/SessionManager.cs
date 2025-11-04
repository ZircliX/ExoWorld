using System.Collections.Generic;
using Unity.Services.Multiplayer;
using UnityEngine;

namespace OverBang.GameName.Managers
{
    public class SessionManager
    {
        public static SessionManager Global => GameController.SessionManager;
        
        private ISession activeSession;

        public async Awaitable<ISession> CreateOrJoinSession(string sessionId, SessionOptions options)
        {
            activeSession = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionId, options);
            return activeSession;
        }
        
        public async Awaitable<IHostSession> CreateSession(SessionOptions options)
        {
            activeSession = await MultiplayerService.Instance.CreateSessionAsync(options);
            return (IHostSession)activeSession;
        }
        
        public async Awaitable<ISession> JoinSessionByID(string sessionID)
        {
            activeSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID);
            return activeSession;
        }
        
        public async Awaitable<ISession> JoinSessionByCode(string code)
        {
            activeSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(code);
            return activeSession;
        }
        
        public async Awaitable<IList<ISessionInfo>> QuerySessions(QuerySessionsOptions options)
        {
            options ??= new QuerySessionsOptions();
            QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(options);
            return results.Sessions;
        }
        
        public async Awaitable KickPlayer(string playerID)
        {
            if (!activeSession.IsHost) return;
            await activeSession.AsHost().RemovePlayerAsync(playerID);
        }

        public async Awaitable LeaveCurrentSession()
        {
            if (activeSession != null)
            {
                try
                {
                    await activeSession.LeaveAsync();
                }
                catch
                {
                    // Game Closed, Ignore
                }
                finally
                {
                    activeSession = null;
                }
            }
        }
    }
}