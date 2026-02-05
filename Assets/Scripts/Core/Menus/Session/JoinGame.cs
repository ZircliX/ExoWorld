using System;
using OverBang.ExoWorld.Core.Utils;
using UnityEngine;

namespace OverBang.ExoWorld.Core.Menus
{
    public class JoinGame : MonoBehaviour
    {
        [SerializeField] private JoinGameUI joinGameUI;

        private void Awake()
        {
            joinGameUI.OnJoinGameByCodeRequested += OnJoinGameByCodeRequested;
            joinGameUI.OnJoinGameBySessionRequested += OnJoinGameBySessionRequested;
        }

        private void OnDestroy()
        {
            joinGameUI.OnJoinGameByCodeRequested -= OnJoinGameByCodeRequested;
            joinGameUI.OnJoinGameBySessionRequested -= OnJoinGameBySessionRequested;
        }

        private void OnJoinGameByCodeRequested(string code)
        {
            JoinGameByCodeAsync(code);
        }
        
        private void OnJoinGameBySessionRequested(SessionInfo info)
        {
            JoinGameBySessionAsync(info);
        }

        private async void JoinGameByCodeAsync(string code)
        {
            try
            {
                await SessionManager.Global.JoinSessionByCode(code);
                joinGameUI.OnJoinedGame?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        
        private async void JoinGameBySessionAsync(SessionInfo info)
        {
            try
            {
                await SessionManager.Global.JoinSessionByID(info.sessionId);
                joinGameUI.OnJoinedGame?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}