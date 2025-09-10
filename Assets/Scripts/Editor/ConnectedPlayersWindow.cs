using System;
using System.Collections.Generic;
using OverBang.GameName.Managers;
using OverBang.GameName.Network;
using OverBang.GameName.Player;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class ConnectedPlayersWindow : EditorWindow
    {
        private Vector2 scrollPos;
        private Dictionary<ulong, bool> foldouts = new Dictionary<ulong, bool>();
        
        [MenuItem("Tools/Networking/Connected Players")]
        public static void ShowWindow() => GetWindow<ConnectedPlayersWindow>("Connected Players");

        public void OnInspectorUpdate() => Repaint();

        private bool CheckForValidation() => EditorApplication.isPlaying && PlayerManager.HasInstance && PlayerManager.Instance.IsSpawned;

        private void OnGUI()
        {
            if (!CheckForValidation())
            {
                EditorGUILayout.HelpBox("No active network connections found. Cannot fetch data.", MessageType.Warning);
                return;
            }

            GUILayout.Label("SESSION", EditorStyles.boldLabel);

            // Join code row
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Join Code : {HostJoin.JoinCode}", GUILayout.Width(150));
            if (GUILayout.Button("COPY", GUILayout.Width(75)))
            {
                EditorGUIUtility.systemCopyBuffer = HostJoin.JoinCode;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Label($"Total Players: {PlayerManager.Instance.Players.Count} / 4", GUILayout.Width(150));

            EditorGUILayout.Space();

            // --- Batch Actions ---
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("✅ Ready All", GUILayout.Width(100)))
            {
                foreach (KeyValuePair<ulong, PlayerController> kvp in PlayerManager.Instance.Players)
                {
                    PlayerController p = kvp.Value;
                    if (!p.PlayerNetwork.IsReady.Value)
                        p.PlayerNetwork.RequestSetReadyRpc(true);
                }
            }
            if (GUILayout.Button("❌ Unready All", GUILayout.Width(100)))
            {
                foreach (KeyValuePair<ulong, PlayerController> kvp in PlayerManager.Instance.Players)
                {
                    PlayerController p = kvp.Value;
                    if (p.PlayerNetwork.IsReady.Value)
                        p.PlayerNetwork.RequestSetReadyRpc(false);
                }
            }
            if (GUILayout.Button("🚪 Kick All Clients", GUILayout.Width(150)))
            {
                foreach (KeyValuePair<ulong, PlayerController> kvp in PlayerManager.Instance.Players)
                {
                    if (!kvp.Value.PlayerNetwork.IsHost) // don’t kick host
                        NetworkManager.Singleton.DisconnectClient(kvp.Key);
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // --- Scroll area with resizable height ---
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Width(position.width), GUILayout.Height(position.height));

            EditorGUILayout.BeginVertical();
            foreach (KeyValuePair<ulong, PlayerController> players in PlayerManager.Instance.Players)
            {
                EditorGUILayout.Space();
                DisplayPlayerData(players.Value, players.Key);
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndScrollView();
        }


        private void DisplayPlayerData(PlayerController player, ulong playerID)
        {
            // Highlight local player
            Color prevBg = GUI.backgroundColor;
            GUI.backgroundColor = player.PlayerNetwork.IsOwner ? new Color(0.25f, 0.45f, 0.25f) : // greenish tint
                new Color(0.2f, 0.2f, 0.2f);

            EditorGUILayout.BeginVertical("box");
            GUI.backgroundColor = prevBg;

            // --- Collapsible Header ---
            foldouts.TryAdd(playerID, true);
            foldouts[playerID] = EditorGUILayout.Foldout(
                foldouts[playerID], 
                $"Player {playerID} {(player.PlayerNetwork.IsOwner ? "(Local)" : "")}", 
                true, 
                new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold }
            );

            if (foldouts[playerID])
            {
                // --- Ready State ---
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Ready Status: ", GUILayout.Width(100));
                bool isReady = player.PlayerNetwork.IsReady.Value;
                Texture readyIcon = EditorGUIUtility.IconContent(isReady ? "TestPassed" : "TestFailed").image;
                GUILayout.Label(new GUIContent(isReady ? "Ready" : "Not Ready", readyIcon));
                if (GUILayout.Button("Toggle", GUILayout.Width(75))) {
                    player.PlayerNetwork.RequestSetReadyRpc(!isReady);
                }
                EditorGUILayout.EndHorizontal();

                // --- Transform ---
                EditorGUILayout.Space();
                GUILayout.Label($"Pos: {player.PlayerNetwork.PlayerState.Value.Position:F1}");
                GUILayout.Label($"RotY: {player.PlayerNetwork.PlayerState.Value.RotationY:F1} °");

                // --- Network ---
                EditorGUILayout.Space();
                GUILayout.Label("Network", EditorStyles.boldLabel);

                // Authority
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Authority: ", GUILayout.Width(100));
                bool isHost = player.PlayerNetwork.IsHost;
                GUILayout.Label(new GUIContent(isHost ? "Host" : "Client"));
                EditorGUILayout.EndHorizontal();

                // Client ID
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("ClientID: ", GUILayout.Width(100));
                GUILayout.Label($"{player.PlayerNetwork.OwnerClientId}");
                EditorGUILayout.EndHorizontal();

                // Ping
                EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Ping: ", GUILayout.Width(100));
                float playerPing = PlayerManager.Instance.PingManager.GetPlayerPing(playerID);
                double ping = Math.Round(playerPing, 2);

                // Ping dot color
                Color pingColor = ping < 50 ? Color.green : ping < 100 ? new Color(1f, 0.64f, 0f) : Color.red;
                GUIStyle pingStyle = new GUIStyle() { normal = { textColor = pingColor } };
                
                GUILayout.Label(new GUIContent($"{ping} ms"), pingStyle);
                EditorGUILayout.EndHorizontal();

                // --- Tools ---
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Localize", GUILayout.Width(75))) {
                    Selection.activeObject = player.gameObject;
                }
                if (GUILayout.Button("Kick", GUILayout.Width(75))) {
                    NetworkManager.Singleton.DisconnectClient(playerID);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }
    }
}