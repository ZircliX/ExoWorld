#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLayers.Editor
{
    public class SceneLayerViewsPanel : EditorWindow
    {
        private SceneLayerDatabase m_database;
        private string m_currentScenePath = "";
        private Vector2 m_scrollPosition;

        [MenuItem("Window/Scene Layers/Views Panel", false, 1001)]
        public static void ShowWindow()
        {
            var win = GetWindow<SceneLayerViewsPanel>();
            win.titleContent = new GUIContent("Layer Views");
            win.minSize = new Vector2(200, 50);
            win.maxSize = new Vector2(4000, 85);
            win.Show();
        }

        private void OnEnable()
        {
            LoadDatabaseForCurrentScene();
            EditorSceneManager.sceneOpened += OnSceneChanged;
            EditorSceneManager.newSceneCreated += OnNewSceneCreated;
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened -= OnSceneChanged;
            EditorSceneManager.newSceneCreated -= OnNewSceneCreated;
        }

        private void OnSceneChanged(Scene scene, OpenSceneMode mode)
        {
            EditorApplication.delayCall += () => {
                LoadDatabaseForCurrentScene();
                Repaint();
            };
        }

        private void OnNewSceneCreated(Scene scene, NewSceneSetup setup, NewSceneMode mode)
        {
            EditorApplication.delayCall += () => {
                LoadDatabaseForCurrentScene();
                Repaint();
            };
        }

        private void LoadDatabaseForCurrentScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            string scenePath = activeScene.path;

            if (m_currentScenePath == scenePath && m_database != null) return;

            m_currentScenePath = scenePath;

            if (string.IsNullOrEmpty(scenePath))
            {
                m_database = ScriptableObject.CreateInstance<SceneLayerDatabase>();
                return;
            }

            string sceneDir = System.IO.Path.GetDirectoryName(scenePath);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            string dbPath = $"{sceneDir}/{sceneName}_SceneLayers.asset";

            m_database = AssetDatabase.LoadAssetAtPath<SceneLayerDatabase>(dbPath);

            if (m_database == null)
            {
                m_database = ScriptableObject.CreateInstance<SceneLayerDatabase>();
            }
        }

        private void OnGUI()
        {
            if (m_database == null)
            {
                LoadDatabaseForCurrentScene();
            }

            var activeScene = SceneManager.GetActiveScene();
            string sceneName = string.IsNullOrEmpty(activeScene.name) ? "Untitled Scene" : activeScene.name;
            bool isSceneDirty = activeScene.isDirty;

            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                string displayName = isSceneDirty ? $"{sceneName}*" : sceneName;
                GUILayout.Label(displayName, EditorStyles.miniLabel);

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Manage", EditorStyles.toolbarButton, GUILayout.Width(60)))
                {
                    var mainWindow = GetWindow<SceneLayerManagerWindow>();
                    mainWindow.Show();
                    mainWindow.Focus();
                }
            }

            if (m_database == null || m_database.views == null || m_database.views.Count == 0)
            {
                EditorGUILayout.Space(4);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    GUILayout.Label("No views yet", EditorStyles.miniLabel);
                    GUILayout.FlexibleSpace();
                }
                return;
            }

            EditorGUILayout.Space(2);
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition, GUIStyle.none, GUIStyle.none);
            DrawViewButtons();
            EditorGUILayout.EndScrollView();
        }

        private void DrawViewButtons()
        {
            const float buttonHeight = 22f;
            const float buttonSpacing = 2f;
            const float sidePadding = 4f;

            float availableWidth = position.width - (sidePadding * 2);
            const float minButtonWidth = 90f;

            int maxButtonsPerRow = Mathf.FloorToInt((availableWidth + buttonSpacing) / (minButtonWidth + buttonSpacing));
            maxButtonsPerRow = Mathf.Max(1, maxButtonsPerRow);

            int totalButtons = m_database.views.Count;
            int numRows = Mathf.CeilToInt((float)totalButtons / maxButtonsPerRow);
            int buttonsPerRow = Mathf.CeilToInt((float)totalButtons / numRows);

            var buttonRows = new List<List<SceneLayerDatabase.LayerView>>();
            var currentRow = new List<SceneLayerDatabase.LayerView>();

            foreach (var view in m_database.views)
            {
                if (currentRow.Count >= buttonsPerRow)
                {
                    buttonRows.Add(currentRow);
                    currentRow = new List<SceneLayerDatabase.LayerView>();
                }
                currentRow.Add(view);
            }

            if (currentRow.Count > 0)
            {
                buttonRows.Add(currentRow);
            }

            for (int rowIndex = 0; rowIndex < buttonRows.Count; rowIndex++)
            {
                var row = buttonRows[rowIndex];
                var rowRect = EditorGUILayout.GetControlRect(false, buttonHeight);
                var paddedRect = new Rect(rowRect.x + sidePadding, rowRect.y, rowRect.width - (sidePadding * 2), rowRect.height);

                float totalSpacing = (row.Count - 1) * buttonSpacing;
                float buttonWidth = (paddedRect.width - totalSpacing) / row.Count;
                float currentX = paddedRect.x;

                for (int buttonIndex = 0; buttonIndex < row.Count; buttonIndex++)
                {
                    var view = row[buttonIndex];

                    if (buttonIndex > 0)
                    {
                        currentX += buttonSpacing;
                    }

                    var buttonRect = new Rect(currentX, paddedRect.y, buttonWidth, buttonHeight);
                    DrawViewButton(buttonRect, view, buttonWidth);

                    currentX += buttonWidth;
                }

                if (rowIndex < buttonRows.Count - 1)
                {
                    EditorGUILayout.Space(2);
                }
            }
        }

        private void DrawViewButton(Rect buttonRect, SceneLayerDatabase.LayerView view, float buttonWidth)
        {
            var baseColor = view.color;
            var hoverColor = new Color(
                Mathf.Min(baseColor.r + 0.15f, 1f),
                Mathf.Min(baseColor.g + 0.15f, 1f),
                Mathf.Min(baseColor.b + 0.15f, 1f),
                1f
            );

            var buttonBgColor = buttonRect.Contains(Event.current.mousePosition) ? hoverColor : baseColor;
            EditorGUI.DrawRect(buttonRect, buttonBgColor);

            var borderColor = new Color(
                Mathf.Min(baseColor.r + 0.2f, 1f),
                Mathf.Min(baseColor.g + 0.2f, 1f),
                Mathf.Min(baseColor.b + 0.2f, 1f),
                1f
            );

            EditorGUI.DrawRect(new Rect(buttonRect.x, buttonRect.y, buttonRect.width, 1), borderColor);
            EditorGUI.DrawRect(new Rect(buttonRect.x, buttonRect.yMax - 1, buttonRect.width, 1), borderColor);
            EditorGUI.DrawRect(new Rect(buttonRect.x, buttonRect.y, 1, buttonRect.height), borderColor);
            EditorGUI.DrawRect(new Rect(buttonRect.xMax - 1, buttonRect.y, 1, buttonRect.height), borderColor);

            string displayText = view.viewName;
            if (buttonWidth < 50f)
            {
                var words = view.viewName.Split(' ');
                if (words.Length > 1)
                {
                    displayText = string.Join("", words.Select(w => w.Length > 0 ? w[0].ToString() : ""));
                }
                else if (view.viewName.Length > 3)
                {
                    displayText = view.viewName.Substring(0, 3) + "…";
                }
            }
            else if (buttonWidth < 80f)
            {
                if (view.viewName.Length > 8)
                {
                    displayText = view.viewName.Substring(0, 7) + "…";
                }
            }

            var textStyle = new GUIStyle(EditorStyles.label);
            textStyle.normal.textColor = Color.white;
            textStyle.fontSize = buttonWidth < 80f ? 10 : 11;
            textStyle.fontStyle = FontStyle.Bold;
            textStyle.alignment = TextAnchor.MiddleCenter;
            GUI.Label(buttonRect, displayText, textStyle);

            if (GUI.Button(buttonRect, "", GUIStyle.none))
            {
                ApplyLayerView(view);
            }
        }

        private void ApplyLayerView(SceneLayerDatabase.LayerView view)
        {
            if (view == null || view.states == null || m_database == null) return;

            Undo.RecordObject(m_database, "Apply Layer View");

            foreach (var state in view.states)
            {
                var layer = m_database.layers.FirstOrDefault(l => l.guid == state.layerGuid);
                if (layer == null) continue;

                layer.defaultVisible = state.visible;
                layer.defaultPickable = state.pickable;
                SceneLayerController.SetLayerVisibility(m_database, layer, state.visible);
                SceneLayerController.SetLayerPickable(m_database, layer, state.pickable);
            }

            EditorUtility.SetDirty(m_database);
            EditorApplication.RepaintHierarchyWindow();

            var existingWindows = Resources.FindObjectsOfTypeAll<SceneLayerManagerWindow>();
            if (existingWindows.Length > 0)
            {
                existingWindows[0].Repaint();
            }
        }
    }
}
#endif