using System;
using System.Collections.Generic;
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

            GUILayout.Label($"SESSION", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Join Code : {HostJoin.JoinCode}", GUILayout.Width(150));
            if (GUILayout.Button("COPY", GUILayout.Width(75)))
            {
                EditorGUIUtility.systemCopyBuffer = HostJoin.JoinCode;
            }
            EditorGUILayout.EndHorizontal();
            
            GUILayout.Label($"Total Players: {PlayerManager.Instance.Players.Count} / 4", GUILayout.Width(150));
            
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
            //Player Object
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Player {playerID} Data", EditorStyles.boldLabel);
            if (GUILayout.Button("Localize", GUILayout.Width(75)))
            {
                Selection.activeObject = player.gameObject;
            }
            EditorGUILayout.EndHorizontal();
    
            //Ready
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Ready Status: ", GUILayout.Width(100));
            GUILayout.Label(player.PlayerNetwork.IsReady.Value.ToString());
            if (GUILayout.Button("Set Ready", GUILayout.Width(75))) {
                player.PlayerNetwork.RequestSetReadyRpc(!player.PlayerNetwork.IsReady.Value);
            }
            EditorGUILayout.EndHorizontal();
            
            //Transform
            EditorGUILayout.Space();
            GUILayout.Label($"Pos: {player.PlayerNetwork.PlayerState.Value.Position:F1}");
            GUILayout.Label($"RotY: {player.PlayerNetwork.PlayerState.Value.RotationY:F1} °");

            EditorGUILayout.Space();
            GUILayout.Label($"Network", EditorStyles.boldLabel);
            
            //Authority
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Authority: ", GUILayout.Width(100));
            GUILayout.Label(player.PlayerNetwork.IsHost ? "Host" : "Client");
            EditorGUILayout.EndHorizontal();
            
            //Client ID
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("ClientID: ", GUILayout.Width(100));
            GUILayout.Label($"{player.PlayerNetwork.OwnerClientId}");
            EditorGUILayout.EndHorizontal();
            
            //Ping
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Ping: ", GUILayout.Width(100));
            float playerPing = PlayerManager.Instance.PingManager.GetPlayerPing(playerID);
            double ping = Math.Round(playerPing, 2);
            GUIStyle style = new GUIStyle() { normal = { textColor = ping < 50 ? Color.green : ping < 100 ? Color.orange : Color.red } };
            GUILayout.Label($"{ping} ms", style);
            EditorGUILayout.EndHorizontal();
        }
    }
}