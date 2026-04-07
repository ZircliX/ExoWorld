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

        public ISession ActiveSession { get; private set; }
        public IPlayer CurrentPlayer => ActiveSession?.CurrentPlayer;

        private bool isCreatingSession = false;

        public bool IsHost()
        {
            return ActiveSession.Players[0].Id == CurrentPlayer.Id;
        }

        // ── Shared helpers ────────────────────────────────────────────────────

        /// <summary>
        /// Leaves any active session and waits for NetworkManager to fully stop.
        /// Must be called before creating or joining any new session.
        /// </summary>
        private async Awaitable EnsureNetworkShutdown()
        {
            if (ActiveSession != null)
            {
                try { await ActiveSession.LeaveAsync(); }
                catch (Exception e) { Debug.LogWarning($"[SessionManager] LeaveAsync failed: {e.Message}"); }
                finally { ActiveSession = null; }
            }

            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.Shutdown();
                while (NetworkManager.Singleton.IsListening)
                    await Awaitable.NextFrameAsync();
            }
        }

        // ── Session creation / joining ────────────────────────────────────────

        public async Awaitable<ISession> CreateOrJoinSession(string sessionId, SessionOptions options)
        {
            await EnsureNetworkShutdown();

            ActiveSession = await MultiplayerService.Instance.CreateOrJoinSessionAsync(sessionId, options);
            await SetPlayerName();

            OnSessionChanged?.Invoke(ActiveSession);
            return ActiveSession;
        }

        public async Awaitable<IHostSession> CreateSession(SessionOptions options)
        {
            if (isCreatingSession) return null;
            isCreatingSession = true;

            try
            {
                await EnsureNetworkShutdown();

                ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
                await SetPlayerName();
                OnSessionChanged?.Invoke(ActiveSession);
                return (IHostSession)ActiveSession;
            }
            finally
            {
                isCreatingSession = false;
            }
        }

        public async Awaitable<ISession> JoinSessionByID(string sessionID)
        {
            await EnsureNetworkShutdown();

            ActiveSession = await MultiplayerService.Instance.JoinSessionByIdAsync(sessionID);
            await SetPlayerName();

            OnSessionChanged?.Invoke(ActiveSession);
            return ActiveSession;
        }

        public async Awaitable<ISession> JoinSessionByCode(string password)
        {
            await EnsureNetworkShutdown();

            ActiveSession = await MultiplayerService.Instance.JoinSessionByCodeAsync(password);
            await SetPlayerName();

            OnSessionChanged?.Invoke(ActiveSession);
            return ActiveSession;
        }

        // ── Session utilities ─────────────────────────────────────────────────

        public async Awaitable<IList<ISessionInfo>> QuerySessions(QuerySessionsOptions options)
        {
            options ??= new QuerySessionsOptions();
            QuerySessionsResults results = await MultiplayerService.Instance.QuerySessionsAsync(options);
            return results.Sessions;
        }

        public async Awaitable SetPlayerName()
        {
            await ActiveSession.CurrentPlayer.UpdatePlayerProperty(
                GameMetrics.Global.ConstID.PlayerPropertyPlayerName,
                PlayerPrefs.GetString(GameMetrics.Global.ConstID.PlayerPropertyPlayerName));
        }

        public async Awaitable KickPlayer(string playerID)
        {
            if (!ActiveSession.IsHost) return;
            await ActiveSession.AsHost().RemovePlayerAsync(playerID);
        }

        public async Awaitable LeaveCurrentSession()
        {
            if (ActiveSession == null) return;

            try
            {
                await ActiveSession.LeaveAsync();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[SessionManager] LeaveSession failed cleanly: {e.Message}");

                if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening)
                    NetworkManager.Singleton.Shutdown();
            }
            finally
            {
                ActiveSession = null;
                OnSessionChanged?.Invoke(null);
            }
        }
    }
}