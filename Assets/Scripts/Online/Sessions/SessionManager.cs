using System.Collections.Generic;
using UnityEngine;

namespace OverBang.GameName.Sessions
{
    public class SessionManager : MonoBehaviour
    {
        public static SessionManager Instance { get; private set; }
        private Dictionary<ulong, Client> sessions;

        private void Awake()
        {
            sessions = new Dictionary<ulong, Client>(4);
            Instance = this;
        }

        public void RegisterSession(Client session) 
            => sessions[session.ClientId] = session;

        public void UnregisterSession(Client session) 
            => sessions.Remove(session.ClientId);

        public Client GetSession(ulong clientId)
            => sessions.GetValueOrDefault(clientId);
    }
}