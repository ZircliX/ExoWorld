#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLayers.Editor
{
    public class SceneLayerFloatingToolbar : EditorWindow
    {
        private SceneLayerDatabase m_database;
        private string m_currentScenePath = "";

        private int m_dragControlId;
        private int m_resizeControlId;
        private bool m_isDragging;
        private bool m_isResizing;
        private Vector2 m_dragStartScreenPos;
        private Rect m_dragStartRect;
        private Vector2 m_resizeStartScreenPos;
        private Rect m_resizeStartRect;

        private const float MIN_BUTTON_WIDTH = 70f;
        private const float BUTTON_HEIGHT = 24f;
        private const float ROW_SPACING = 2f;
        private const float MIN_WINDOW_WIDTH = 80f;
        private const float DRAG_HANDLE_HEIGHT = 12f;
        private const float RESIZE_HANDLE_SIZE = 16f;
        private const float BUTTON_SPACING = 4f;
        private const float SIDE_PADDING = 4f;

        private float GetMinWindowWidth()
        {
            return (SIDE_PADDING * 2) + MIN_BUTTON_WIDTH;
        }

        [MenuItem("Tools/Scene Layers/Views Overlay", false, 1002)]
        public static void ShowWindow()
        {
            var existingWindows = Resources.FindObjectsOfTypeAll<SceneLayerFloatingToolbar>();
            foreach (var w in existingWindows)
            {
                w.Close();
            }

            var win = CreateInstance<SceneLayerFloatingToolbar>();
            win.titleContent = new GUIContent("Layer Views");

            var pos = new Rect(100, 100, 400, 60);
            if (EditorPrefs.HasKey("SceneLayers.FloatingToolbar.X"))
            {
                pos.x = EditorPrefs.GetFloat("SceneLayers.FloatingToolbar.X");
                pos.y = EditorPrefs.GetFloat("SceneLayers.FloatingToolbar.Y");
                pos.width = EditorPrefs.GetFloat("SceneLayers.FloatingToolbar.Width", 400);
                pos.height = EditorPrefs.GetFloat("SceneLayers.FloatingToolbar.Height", 60);
            }

            win.minSize = new Vector2(1, 1);
            win.maxSize = new Vector2(4000, 500);
            win.position = pos;
            win.ShowPopup();
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

            EditorPrefs.SetFloat("SceneLayers.FloatingToolbar.X", position.x);
            EditorPrefs.SetFloat("SceneLayers.FloatingToolbar.Y", position.y);
            EditorPrefs.SetFloat("SceneLayers.FloatingToolbar.Width", position.width);
            EditorPrefs.SetFloat("SceneLayers.FloatingToolbar.Height", position.height);
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

        private int CalculateRowCount(float width)
        {
            if (m_database == null || m_database.views == null || m_database.views.Count == 0)
                return 1;

            float availableWidth = width - (SIDE_PADDING * 2);
            int maxButtonsPerRow = Mathf.Max(1, Mathf.FloorToInt((availableWidth + BUTTON_SPACING) / (MIN_BUTTON_WIDTH + BUTTON_SPACING)));

            int totalButtons = m_database.views.Count;
            int numRows = Mathf.CeilToInt((float)totalButtons / maxButtonsPerRow);

            return Mathf.Max(1, numRows);
        }

        private float CalculateMinHeight(float width)
        {
            int numRows = CalculateRowCount(width);
            return DRAG_HANDLE_HEIGHT + 4f + (numRows * BUTTON_HEIGHT) + ((numRows - 1) * ROW_SPACING) + 4f;
        }

        private void OnGUI()
        {
            if (m_database == null)
            {
                LoadDatabaseForCurrentScene();
            }

            m_dragControlId = GUIUtility.GetControlID(FocusType.Passive);
            m_resizeControlId = GUIUtility.GetControlID(FocusType.Passive);

            HandleCloseButton();
            HandleResizing();
            HandleDragging();

            DrawBackground();
            DrawDragHandle();
            DrawToolbar();
            DrawResizeHandle();
        }

        private void HandleCloseButton()
        {
            var e = Event.current;
            var closeRect = new Rect(position.width - 14f, 1f, 12f, 10f);

            if (e.type == EventType.MouseDown && e.button == 0 && closeRect.Contains(e.mousePosition))
            {
                Close();
                e.Use();
                GUIUtility.ExitGUI();
            }
        }

        private void DrawDragHandle()
        {
            var handleRect = new Rect(0, 0, position.width, DRAG_HANDLE_HEIGHT);

            var handleColor = EditorGUIUtility.isProSkin
                ? new Color(0.18f, 0.18f, 0.18f, 1f)
                : new Color(0.68f, 0.68f, 0.68f, 1f);
            EditorGUI.DrawRect(handleRect, handleColor);

            var gripColor = EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.3f)
                : new Color(0f, 0f, 0f, 0.3f);

            float centerX = position.width / 2f;
            float centerY = DRAG_HANDLE_HEIGHT / 2f;

            for (int i = -2; i <= 2; i++)
            {
                EditorGUI.DrawRect(new Rect(centerX + (i * 4f) - 1f, centerY - 1f, 2f, 2f), gripColor);
            }

            var closeRect = new Rect(position.width - 14f, 1f, 12f, 10f);
            var closeColor = closeRect.Contains(Event.current.mousePosition)
                ? (EditorGUIUtility.isProSkin ? new Color(1f, 0.4f, 0.4f, 1f) : new Color(0.8f, 0.2f, 0.2f, 1f))
                : (EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.5f) : new Color(0f, 0f, 0f, 0.5f));

            var style = new GUIStyle(EditorStyles.miniLabel);
            style.fontSize = 9;
            style.fontStyle = FontStyle.Bold;
            style.normal.textColor = closeColor;
            style.alignment = TextAnchor.MiddleCenter;
            GUI.Label(closeRect, "✕", style);
        }
        private void DrawBackground()
        {
            var bgColor = EditorGUIUtility.isProSkin
                ? new Color(0.22f, 0.22f, 0.22f, 1f)
                : new Color(0.76f, 0.76f, 0.76f, 1f);
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), bgColor);

            var borderColor = EditorGUIUtility.isProSkin
                ? new Color(0.1f, 0.1f, 0.1f, 1f)
                : new Color(0.5f, 0.5f, 0.5f, 1f);
            EditorGUI.DrawRect(new Rect(0, 0, position.width, 1), borderColor);
            EditorGUI.DrawRect(new Rect(0, position.height - 1, position.width, 1), borderColor);
            EditorGUI.DrawRect(new Rect(0, 0, 1, position.height), borderColor);
            EditorGUI.DrawRect(new Rect(position.width - 1, 0, 1, position.height), borderColor);
        }
        private void DrawResizeHandle()
        {
            var gripColor = EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.3f)
                : new Color(0f, 0f, 0f, 0.3f);

            float baseX = position.width - 4f;
            float baseY = position.height - 4f;

            EditorGUI.DrawRect(new Rect(baseX - 2f, baseY - 2f, 2f, 2f), gripColor);

            EditorGUI.DrawRect(new Rect(baseX - 5f, baseY - 2f, 2f, 2f), gripColor);
            EditorGUI.DrawRect(new Rect(baseX - 2f, baseY - 5f, 2f, 2f), gripColor);

            EditorGUI.DrawRect(new Rect(baseX - 8f, baseY - 2f, 2f, 2f), gripColor);
            EditorGUI.DrawRect(new Rect(baseX - 5f, baseY - 5f, 2f, 2f), gripColor);
            EditorGUI.DrawRect(new Rect(baseX - 2f, baseY - 8f, 2f, 2f), gripColor);

            var handleRect = new Rect(position.width - RESIZE_HANDLE_SIZE, position.height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE);
            EditorGUIUtility.AddCursorRect(handleRect, MouseCursor.ResizeUpLeft);
        }

        private void HandleDragging()
        {
            var e = Event.current;
            var dragRect = new Rect(0, 0, position.width, DRAG_HANDLE_HEIGHT);

            switch (e.GetTypeForControl(m_dragControlId))
            {
                case EventType.MouseDown:
                    if (e.button == 0 && dragRect.Contains(e.mousePosition))
                    {
                        GUIUtility.hotControl = m_dragControlId;
                        m_isDragging = true;
                        m_dragStartScreenPos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                        m_dragStartRect = position;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == m_dragControlId && m_isDragging)
                    {
                        Vector2 currentScreenPos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                        Vector2 delta = currentScreenPos - m_dragStartScreenPos;

                        var newPos = m_dragStartRect;
                        newPos.position += delta;
                        position = newPos;

                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == m_dragControlId)
                    {
                        GUIUtility.hotControl = 0;
                        m_isDragging = false;
                        e.Use();
                    }
                    break;
            }
        }

        private void HandleResizing()
        {
            var e = Event.current;
            var resizeRect = new Rect(position.width - RESIZE_HANDLE_SIZE, position.height - RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE, RESIZE_HANDLE_SIZE);

            switch (e.GetTypeForControl(m_resizeControlId))
            {
                case EventType.MouseDown:
                    if (e.button == 0 && resizeRect.Contains(e.mousePosition))
                    {
                        GUIUtility.hotControl = m_resizeControlId;
                        m_isResizing = true;
                        m_resizeStartScreenPos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                        m_resizeStartRect = position;
                        e.Use();
                    }
                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == m_resizeControlId && m_isResizing)
                    {
                        Vector2 currentScreenPos = GUIUtility.GUIToScreenPoint(e.mousePosition);
                        Vector2 delta = currentScreenPos - m_resizeStartScreenPos;

                        Rect newPos = m_resizeStartRect;
                        float minWidth = GetMinWindowWidth();
                        newPos.width = Mathf.Max(minWidth, m_resizeStartRect.width + delta.x);

                        float minHeight = CalculateMinHeight(newPos.width);
                        newPos.height = Mathf.Max(minHeight, m_resizeStartRect.height + delta.y);

                        position = newPos;
                        e.Use();
                        Repaint();
                    }
                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == m_resizeControlId)
                    {
                        GUIUtility.hotControl = 0;
                        m_isResizing = false;
                        e.Use();
                    }
                    break;
            }
        }

        private void DrawToolbar()
        {
            if (m_database == null || m_database.views == null || m_database.views.Count == 0)
            {
                var labelRect = new Rect(0, DRAG_HANDLE_HEIGHT, position.width, position.height - DRAG_HANDLE_HEIGHT - RESIZE_HANDLE_SIZE);
                var style = new GUIStyle(EditorStyles.miniLabel);
                style.alignment = TextAnchor.MiddleCenter;
                GUI.Label(labelRect, "No views yet", style);
                return;
            }

            float availableWidth = position.width - (SIDE_PADDING * 2);
            int maxButtonsPerRow = Mathf.Max(1, Mathf.FloorToInt((availableWidth + BUTTON_SPACING) / (MIN_BUTTON_WIDTH + BUTTON_SPACING)));

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

            float yOffset = DRAG_HANDLE_HEIGHT + 4f;

            for (int rowIndex = 0; rowIndex < buttonRows.Count; rowIndex++)
            {
                var row = buttonRows[rowIndex];

                float totalSpacing = (row.Count - 1) * BUTTON_SPACING;
                float buttonWidth = (availableWidth - totalSpacing) / row.Count;
                float currentX = SIDE_PADDING;

                for (int buttonIndex = 0; buttonIndex < row.Count; buttonIndex++)
                {
                    var view = row[buttonIndex];

                    if (buttonIndex > 0)
                    {
                        currentX += BUTTON_SPACING;
                    }

                    var buttonRect = new Rect(currentX, yOffset, buttonWidth, BUTTON_HEIGHT);
                    DrawViewButton(buttonRect, view, buttonWidth);

                    currentX += buttonWidth;
                }

                yOffset += BUTTON_HEIGHT + ROW_SPACING;
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
                Mathf.Min(baseColor.r + 0.3f, 1f),
                Mathf.Min(baseColor.g + 0.3f, 1f),
                Mathf.Min(baseColor.b + 0.3f, 1f),
                1f
            );

            EditorGUI.DrawRect(new Rect(buttonRect.x, buttonRect.y, buttonRect.width, 1), borderColor);
            EditorGUI.DrawRect(new Rect(buttonRect.x, buttonRect.yMax - 1, buttonRect.width, 1), borderColor);
            EditorGUI.DrawRect(new Rect(buttonRect.x, buttonRect.y, 1, buttonRect.height), borderColor);
            EditorGUI.DrawRect(new Rect(buttonRect.xMax - 1, buttonRect.y, 1, buttonRect.height), borderColor);

            string displayText = view.viewName;
            if (buttonWidth < 80f && view.viewName.Length > 8)
            {
                displayText = view.viewName.Substring(0, 7) + "…";
            }

            var textStyle = new GUIStyle(EditorStyles.label);
            textStyle.normal.textColor = Color.white;
            textStyle.fontSize = 11;
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

            if (view.hasCameraData)
            {
                var sceneView = SceneView.lastActiveSceneView;
                if (sceneView != null)
                {
                    sceneView.pivot = view.cameraPosition;
                    sceneView.rotation = view.cameraRotation;
                    sceneView.size = view.cameraSize;
                    sceneView.Repaint();
                }
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