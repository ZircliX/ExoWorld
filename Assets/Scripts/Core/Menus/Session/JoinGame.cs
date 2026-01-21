using System;
using UnityEngine;

namespace OverBang.ExoWorld.Core
{
    public class JoinGame : MonoBehaviour
    {
        [SerializeField] private JoinGameUI joinGameUI;

        private void Awake()
        {
            joinGameUI.OnJoinGameRequested += OnJoinGameRequested;
        }

        private void OnDestroy()
        {
            joinGameUI.OnJoinGameRequested -= OnJoinGameRequested;
        }

        private void OnJoinGameRequested(string sessionID, string sessionPassword)
        {
            JoinGameAsync(sessionID, sessionPassword);
        }

        private async void JoinGameAsync(string sessionID, string sessionPassword)
        {
            try
            {
                // Join public
                if (!string.IsNullOrEmpty(sessionID))
                {
                    await SessionManager.Global.JoinSessionByID(sessionID);
                    await Awaitable.WaitForSecondsAsync(0.2f);
                    joinGameUI.OnJoinedGame?.Invoke();
                }

                // Join private
                if (!string.IsNullOrEmpty(sessionPassword))
                {
                    await SessionManager.Global.JoinSessionByPassword(sessionPassword);
                    await Awaitable.WaitForSecondsAsync(0.2f);
                    joinGameUI.OnJoinedGame?.Invoke();
                }
                
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}