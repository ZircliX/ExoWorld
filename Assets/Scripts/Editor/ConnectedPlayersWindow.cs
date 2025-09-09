using System.Collections.Generic;
using System.Globalization;
using OverBang.GameName.Managers;
using OverBang.GameName.Network;
using OverBang.GameName.Player;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ConnectedPlayersWindow : EditorWindow
    {
        private Vector2 scrollPos;
        
        [MenuItem("Tools/Networking/Connected Players")]
        public static void ShowWindow()
        {
            GetWindow<ConnectedPlayersWindow>("Connected Players");
        }
        
        public void OnInspectorUpdate()
        {
            // This will only get called 10 times per second.
            Repaint();
        }

        private void OnGUI()
        {
            if (!CheckForValidation())
            {
                EditorGUILayout.HelpBox("No active network connections found. Cannot fetch data.", MessageType.Warning);
                return;
            }

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Join Code : {HostJoin.JoinCode}", GUILayout.Width(150));
            if (GUILayout.Button("COPY", GUILayout.Width(75)))
            {
                EditorGUIUtility.systemCopyBuffer = HostJoin.JoinCode;
            }
            EditorGUILayout.EndHorizontal();
            
            // Begin scroll view
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));
            
            foreach (KeyValuePair<ulong, PlayerController> players in PlayerManager.Instance.Players)
            {
                EditorGUILayout.Space();

                // Player box
                EditorGUILayout.BeginVertical("box");
                DisplayPlayerData(players.Value, players.Key);
                EditorGUILayout.EndVertical();

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }

        private bool CheckForValidation()
        {
            return EditorApplication.isPlaying && PlayerManager.HasInstance && PlayerManager.Instance.IsSpawned;
        }

        private void DisplayPlayerData(PlayerController player, ulong playerID)
        {
            GUILayout.Label($"Player {playerID} Data", EditorStyles.boldLabel);
    
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Ready Status: ", GUILayout.Width(100));
            GUILayout.Label(player.PlayerNetwork.IsReady.Value.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Ping: ", GUILayout.Width(100));
            float playerPing = PlayerManager.Instance.PingManager.GetPlayerPing(playerID);
            string ping = Mathf.RoundToInt(playerPing).ToString(CultureInfo.InvariantCulture);
            GUILayout.Label(ping);
            EditorGUILayout.EndHorizontal();
        }
    }
}