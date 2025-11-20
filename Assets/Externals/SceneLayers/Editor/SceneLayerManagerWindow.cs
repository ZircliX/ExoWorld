#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace SceneLayers.Editor
{
    public class SceneLayerManagerWindow : EditorWindow
    {
        private const string FOLDOUT_PREF_PREFIX = "SceneLayers.Foldout.";
        private const string SELECTED_LAYER_PREFIX = "SceneLayers.SelectedLayer";
        private const string SHOW_VIEW_MANAGER_PREFIX = "SceneLayers.ShowViewManager";
        private const string ORDER_PREF_PREFIX = "SceneLayers.ObjectOrder.";
        private const string PALETTE_INDEX_PREFIX = "SceneLayers.NextColorIndex";
        private const string SESSION_VERSION_KEY = "SceneLayers.SessionVersion";
        private const string SESSION_CACHE_VALID_KEY = "SceneLayers.CacheValid";
        private const string SESSION_INSTANCE_CACHE_VALID_KEY = "SceneLayers.InstanceCacheValid";
        private const int SESSION_STATE_VERSION = 2;

        private Dictionary<string, List<GameObject>> m_frameCache = new Dictionary<string, List<GameObject>>();
        private int m_lastCacheFrame = -1;

        private const float m_baseGap = 4f;
        private const float m_foldW = 16f;
        private const double m_liveAssignDebounceSec = 0.15;
        private const double m_hoverExpandDelaySec = 0.3f;

        private static readonly Color[] m_DefaultPalette = new[]
        {
            new Color(0.90f, 0.25f, 0.22f),
            new Color(0.13f, 0.59f, 0.95f),
            new Color(0.30f, 0.69f, 0.31f),
            new Color(1.00f, 0.76f, 0.03f),
            new Color(1.00f, 0.34f, 0.13f),
            new Color(0.62f, 0.36f, 0.71f),
            new Color(0.00f, 0.74f, 0.83f),
            new Color(0.65f, 0.85f, 0.35f),
            new Color(0.95f, 0.35f, 0.55f),
        };

        private static SceneLayerManagerWindow m_instance;
        public static Dictionary<string, HashSet<GameObject>> m_layerObjectCache = new Dictionary<string, HashSet<GameObject>>();
        private static bool m_cacheValid = false;

        private static GUIStyle m_flatLayerBoxStyle;
        private static GUIStyle FlatLayerBoxStyle
        {
            get
            {
                if (m_flatLayerBoxStyle == null)
                {
                    m_flatLayerBoxStyle = new GUIStyle(GUIStyle.none);
                    m_flatLayerBoxStyle.padding = new RectOffset(4, 4, 2, 2);
                }
                return m_flatLayerBoxStyle;
            }
        }

        private SceneLayerDatabase m_database;
        private string m_currentScenePath = "";

        public SceneLayerDatabase database { get { return m_database; } }
        public bool hasActiveSearch { get { return m_hasActiveSearch; } }
        public string selectedLayerGuid { get { return m_selectedLayerGuid; } }
        public bool showViewManager { get { return m_showViewManager; } }
        public bool isDraggingLayer { get { return m_draggingLayer; } }
        public bool isDraggingObject { get { return m_draggingObj; } }

        private Vector2 m_scrollPosition;
        private bool m_showViewManager = false;
        private bool m_repaintRequested = false;
        private Rect[] m_headerRects = Array.Empty<Rect>();
        private readonly Dictionary<string, List<Rect>> m_childRowRects = new Dictionary<string, List<Rect>>();
        private readonly Dictionary<string, float> m_layerChildrenHeights = new Dictionary<string, float>();

        private readonly Dictionary<string, bool> m_layerFoldouts = new Dictionary<string, bool>();
        private string m_selectedLayerGuid;
        private string m_prevSelectedLayerGuid = null;

        private string m_searchFilter = "";
        private bool m_hasActiveSearch = false;
        private HashSet<string> m_matchingLayerGuids = new HashSet<string>();
        private HashSet<GameObject> m_matchingObjects = new HashSet<GameObject>();

        private readonly HashSet<string> m_renamingLayers = new HashSet<string>();
        private readonly Dictionary<string, string> m_renameBuffers = new Dictionary<string, string>();
        private readonly HashSet<string> m_renameTextSelected = new HashSet<string>();
        private readonly HashSet<string> m_renamingViews = new HashSet<string>();
        private readonly Dictionary<string, string> m_viewRenameBuffers = new Dictionary<string, string>();
        private readonly HashSet<string> m_viewRenameTextSelected = new HashSet<string>();

        private GameObject m_hoverObjectForHierarchy = null;
        private GameObject m_prevHoverObjectForHierarchy = null;
        private string m_hoverLayerGuidForHierarchy = null;
        private string m_hoverLayerGuidInPanel = null;
        private string m_dragAssignHoverLayerGuid = null;
        private string m_hoverExpandLayerGuidForDrag = null;
        private double m_hoverExpandStartTime = 0.0;

        private bool m_draggingLayer = false;
        private int m_dragFromIndex = -1;
        private int m_dragInsertIndex = -1;
        private int m_dragPreviewInsertIndex = -1;
        private int m_dragPotentialIndex = -1;
        private int m_dragControlId = -1;
        private bool m_pendingDrop = false;
        private Vector2 m_dragMouseDownPos;
        private Vector2 m_lastDragMouseGuiPos;

        private bool m_draggingObj = false;
        private GameObject m_dragObj;
        private string m_dragObjSrcLayerGuid;
        private int m_dragObjFromIndex = -1;
        private string m_dragObjTargetLayerGuid;
        private int m_dragObjInsertIndex = -1;
        private int m_dragObjPreviewInsertIndex = -1;
        private bool m_dragObjPendingDrop = false;
        private Vector2 m_dragObjMouseDownPos;

        private bool m_extDragActive = false;
        private string m_extDragTargetLayerGuid = null;
        private int m_extDragInsertIndex = -1;
        private string m_deferredExpandLayerGuid = null;

        private readonly Dictionary<string, List<string>> m_orderByLayer = new Dictionary<string, List<string>>();
        private Dictionary<string, int> m_layerObjectCounts = new Dictionary<string, int>();
        private Dictionary<string, List<GameObject>> m_orderedObjectsCache = new Dictionary<string, List<GameObject>>();
        internal Dictionary<int, (bool hidden, bool pickingDisabled)> m_visibilityCache = new Dictionary<int, (bool, bool)>();
        private static Dictionary<string, HashSet<int>> s_layerInstanceIds = new Dictionary<string, HashSet<int>>();
        private static bool s_instanceIdCacheValid = false;

        [InitializeOnLoadMethod]
        private static void InitializeOnLoad()
        {
            int savedVersion = SessionState.GetInt(SESSION_VERSION_KEY, 0);
            if (savedVersion != SESSION_STATE_VERSION)
            {
                SessionState.EraseInt(SESSION_VERSION_KEY);
                SessionState.EraseBool(SESSION_CACHE_VALID_KEY);
                SessionState.EraseBool(SESSION_INSTANCE_CACHE_VALID_KEY);
                SessionState.EraseString("SceneLayers.AllLayerGuids");
                SessionState.SetInt(SESSION_VERSION_KEY, SESSION_STATE_VERSION);
            }
            else
            {
                RestoreInstanceIdCacheFromSessionState();
            }

            EditorApplication.playModeStateChanged -= OnStaticPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnStaticPlayModeStateChanged;
        }
        private static void RestoreInstanceIdCacheFromSessionState()
        {
            string layerGuidsStr = SessionState.GetString("SceneLayers.AllLayerGuids", "");
            if (string.IsNullOrEmpty(layerGuidsStr)) return;

            var layerGuids = layerGuidsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            s_layerInstanceIds.Clear();

            foreach (var guid in layerGuids)
            {
                string key = $"SceneLayers.InstanceIds.{guid}";
                string serialized = SessionState.GetString(key, "");

                if (!string.IsNullOrEmpty(serialized))
                {
                    var instanceIds = new HashSet<int>();
                    foreach (var idStr in serialized.Split(','))
                    {
                        if (int.TryParse(idStr, out int id))
                        {
                            instanceIds.Add(id);
                        }
                    }

                    if (instanceIds.Count > 0)
                    {
                        s_layerInstanceIds[guid] = instanceIds;
                    }
                }
            }

            if (s_layerInstanceIds.Count > 0)
            {
                s_instanceIdCacheValid = true;

                m_layerObjectCache.Clear();

                foreach (var kvp in s_layerInstanceIds)
                {
                    var objects = new HashSet<GameObject>();

                    foreach (var instanceId in kvp.Value)
                    {
                        var obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                        if (obj != null)
                        {
                            objects.Add(obj);
                        }
                    }

                    if (objects.Count > 0)
                    {
                        m_layerObjectCache[kvp.Key] = objects;
                    }
                }

                m_cacheValid = true;
            }
        }

        private static void OnStaticPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    if (m_instance?.m_database != null)
                    {
                        s_layerInstanceIds.Clear();

                        foreach (var kvp in m_layerObjectCache)
                        {
                            var instanceIds = new HashSet<int>();
                            foreach (var go in kvp.Value)
                            {
                                if (go != null)
                                {
                                    instanceIds.Add(go.GetInstanceID());
                                }
                            }

                            if (instanceIds.Count > 0)
                            {
                                s_layerInstanceIds[kvp.Key] = instanceIds;
                            }
                        }

                        s_instanceIdCacheValid = true;
                    }

                    SaveInstanceIdsToSessionState();
                    break;

                case PlayModeStateChange.EnteredPlayMode:
                    break;

                case PlayModeStateChange.ExitingPlayMode:
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    if (m_instance != null && s_layerInstanceIds.Count > 0)
                    {
                        EditorApplication.delayCall += () => {
                            if (m_instance != null)
                            {
                                FastRebuildFromInstanceIds();
                                foreach (var layer in m_instance.m_database.layers)
                                {
                                    if (m_layerObjectCache.TryGetValue(layer.guid, out var objects))
                                    {
                                        string key = $"SceneLayers.InstanceIds.{layer.guid}";
                                        string serialized = SessionState.GetString(key, "");

                                        if (!string.IsNullOrEmpty(serialized))
                                        {
                                            var orderedList = new List<GameObject>();
                                            foreach (var idStr in serialized.Split(','))
                                            {
                                                if (int.TryParse(idStr, out int id))
                                                {
                                                    var obj = EditorUtility.InstanceIDToObject(id) as GameObject;
                                                    if (obj != null)
                                                    {
                                                        orderedList.Add(obj);
                                                    }
                                                }
                                            }

                                            m_instance.m_orderedObjectsCache[layer.guid] = orderedList;
                                        }
                                    }
                                }

                                m_instance.Repaint();
                            }
                        };
                    }
                    else
                    {
                        m_cacheValid = false;
                        s_instanceIdCacheValid = false;
                    }
                    break;
            }
        }

        private static void SaveInstanceIdsToSessionState()
        {
            if (m_instance?.m_database == null) return;

            var layerGuids = string.Join(",", m_instance.m_database.layers.Select(l => l.guid));
            SessionState.SetString("SceneLayers.AllLayerGuids", layerGuids);

            foreach (var layer in m_instance.m_database.layers)
            {
                if (s_layerInstanceIds.TryGetValue(layer.guid, out var instanceIds))
                {
                    List<int> orderedInstanceIds;
                    if (m_instance.m_orderedObjectsCache.TryGetValue(layer.guid, out var orderedObjects))
                    {
                        orderedInstanceIds = orderedObjects
                            .Where(go => go != null)
                            .Select(go => go.GetInstanceID())
                            .ToList();
                    }
                    else
                    {
                        orderedInstanceIds = instanceIds.ToList();
                    }

                    string key = $"SceneLayers.InstanceIds.{layer.guid}";
                    string serialized = string.Join(",", orderedInstanceIds);
                    SessionState.SetString(key, serialized);
                }
            }

            SessionState.SetBool(SESSION_CACHE_VALID_KEY, m_cacheValid);
            SessionState.SetBool(SESSION_INSTANCE_CACHE_VALID_KEY, s_instanceIdCacheValid);
            SessionState.SetInt(SESSION_VERSION_KEY, SESSION_STATE_VERSION);
        }
        private static bool LoadInstanceIdsFromSessionState()
        {
            if (m_instance?.m_database == null) return false;

            int savedVersion = SessionState.GetInt(SESSION_VERSION_KEY, 0);
            if (savedVersion != SESSION_STATE_VERSION) return false;

            bool cacheValid = SessionState.GetBool(SESSION_CACHE_VALID_KEY, false);
            bool instanceCacheValid = SessionState.GetBool(SESSION_INSTANCE_CACHE_VALID_KEY, false);

            if (!cacheValid || !instanceCacheValid) return false;

            s_layerInstanceIds.Clear();

            foreach (var layer in m_instance.m_database.layers)
            {
                string key = $"SceneLayers.InstanceIds.{layer.guid}";
                string serialized = SessionState.GetString(key, "");

                if (!string.IsNullOrEmpty(serialized))
                {
                    var instanceIds = new HashSet<int>();
                    foreach (var idStr in serialized.Split(','))
                    {
                        if (int.TryParse(idStr, out int id))
                        {
                            instanceIds.Add(id);
                        }
                    }

                    if (instanceIds.Count > 0)
                    {
                        s_layerInstanceIds[layer.guid] = instanceIds;
                    }
                }
            }

            m_cacheValid = cacheValid;
            s_instanceIdCacheValid = instanceCacheValid;

            return s_layerInstanceIds.Count > 0;
        }

        private static void ClearSessionState()
        {
            SessionState.EraseString("SceneLayers.AllLayerGuids");

            if (m_instance?.m_database != null)
            {
                foreach (var layer in m_instance.m_database.layers)
                {
                    string key = $"SceneLayers.InstanceIds.{layer.guid}";
                    SessionState.EraseString(key);
                }
            }

            SessionState.EraseBool(SESSION_CACHE_VALID_KEY);
            SessionState.EraseBool(SESSION_INSTANCE_CACHE_VALID_KEY);
        }

        private bool m_liveAssignDirty = false;
        private double m_liveAssignNextTime = 0;
        private int m_lastKnownObjectCount = 0;
        private Dictionary<int, HashSet<string>> m_knownObjectComponents = new Dictionary<int, HashSet<string>>();
        private List<GameObject> m_objectsNeedingAutoAssign = new List<GameObject>();
        private static Color GetNextDefaultColor(bool incrementIndex = true)
        {
            const string PALETTE_SIZE_KEY = "SceneLayers.PaletteSize";
            int storedSize = EditorPrefs.GetInt(PALETTE_SIZE_KEY, -1);
            if (storedSize != m_DefaultPalette.Length)
            {
                EditorPrefs.SetInt(PALETTE_INDEX_PREFIX, 0);
                EditorPrefs.SetInt(PALETTE_SIZE_KEY, m_DefaultPalette.Length);
                Debug.Log($"Scene Layers: Detected palette size change ({storedSize} → {m_DefaultPalette.Length}), reset color index");
            }

            int idx = EditorPrefs.GetInt(PALETTE_INDEX_PREFIX, 0);
            if (idx < 0 || idx >= m_DefaultPalette.Length) idx = 0;

            var c = m_DefaultPalette[idx % m_DefaultPalette.Length];
            if (incrementIndex)
            {
                EditorPrefs.SetInt(PALETTE_INDEX_PREFIX, (idx + 1) % m_DefaultPalette.Length);
            }
            return c;
        }

        internal static Color PeekNextDefaultColor()
        {
            return GetNextDefaultColor(incrementIndex: false);
        }

        internal static void IncrementDefaultColorIndex()
        {
            int idx = EditorPrefs.GetInt(PALETTE_INDEX_PREFIX, 0);
            EditorPrefs.SetInt(PALETTE_INDEX_PREFIX, (idx + 1) % m_DefaultPalette.Length);
        }

        [MenuItem("Window/Scene Layers/Layer Manager")]
        public static void ShowWindow()
        {
            var win = GetWindow<SceneLayerManagerWindow>();
            win.titleContent = new GUIContent("Scene Layers");
            win.Show();
        }

        [MenuItem("Window/Scene Layers/Options…")]
        public static void ShowOptionsWindow()
        {
            SceneLayerOptionsWindow.ShowWindow();
        }

        private void OnEnable()
        {
            m_instance = this;
            m_database = LoadSceneSpecificDatabase();
            LoadAllOrders();
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            m_currentScenePath = activeScene.path;
            RestoreUIState();

            EditorApplication.hierarchyWindowItemOnGUI += OnHierarchyItemGUI;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.update += OnEditorUpdate;
            Selection.selectionChanged += OnSelectionChanged;

            EditorSceneManager.sceneOpened += OnSceneOpened;
            EditorSceneManager.newSceneCreated += OnNewSceneCreated;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;

            if (m_cacheValid && m_layerObjectCache.Count > 0)
            {
                foreach (var layer in m_database.layers)
                {
                    if (m_layerObjectCache.TryGetValue(layer.guid, out var objects))
                    {
                        string key = $"SceneLayers.InstanceIds.{layer.guid}";
                        string serialized = SessionState.GetString(key, "");

                        if (!string.IsNullOrEmpty(serialized))
                        {
                            var orderedList = new List<GameObject>();
                            foreach (var idStr in serialized.Split(','))
                            {
                                if (int.TryParse(idStr, out int id))
                                {
                                    var obj = EditorUtility.InstanceIDToObject(id) as GameObject;
                                    if (obj != null)
                                    {
                                        orderedList.Add(obj);
                                    }
                                }
                            }

                            m_orderedObjectsCache[layer.guid] = orderedList;
                        }
                    }
                }
            }
            else if (!m_cacheValid && !EditorApplication.isPlayingOrWillChangePlaymode)
            {
                var allObjects = SceneLayerController.EnumerateAllSceneObjects().ToList();
                UpdateObjectTracking(allObjects);
                RebuildCacheIfNeeded();
            }
        }
        private void OnDisable()
        {
            SaveAllOrders();
            SaveUIState();

            if (m_instance == this) m_instance = null;
            EditorApplication.hierarchyWindowItemOnGUI -= OnHierarchyItemGUI;

            EditorApplication.hierarchyChanged -= OnHierarchyChanged;
            EditorApplication.update -= OnEditorUpdate;
            Selection.selectionChanged -= OnSelectionChanged;

            EditorSceneManager.sceneOpened -= OnSceneOpened;
            EditorSceneManager.newSceneCreated -= OnNewSceneCreated;
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    SaveAllOrders();
                    if (m_database != null)
                    {
                        EditorUtility.SetDirty(m_database);
                    }
                    break;

                case PlayModeStateChange.EnteredEditMode:
                    break;
            }
        }
        public static void InvalidateCache()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            m_cacheValid = false;
            s_instanceIdCacheValid = false;

            if (m_instance != null)
            {
                m_instance.m_layerObjectCounts.Clear();
                m_instance.m_orderedObjectsCache.Clear();
                m_instance.m_visibilityCache.Clear();
            }
        }

        public static void RebuildCacheIfNeeded()
        {
            if (m_cacheValid && s_instanceIdCacheValid) return;

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            m_layerObjectCache.Clear();
            s_layerInstanceIds.Clear();

            if (m_instance?.m_database == null) return;

            foreach (var layer in m_instance.m_database.layers)
            {
                if (layer.objectGlobalIds == null) continue;

                var objects = new HashSet<GameObject>();
                var instanceIds = new HashSet<int>();

                foreach (var goidStr in layer.objectGlobalIds)
                {
                    if (string.IsNullOrEmpty(goidStr)) continue;

                    if (GlobalObjectId.TryParse(goidStr, out var goid))
                    {
                        var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(goid);
                        if (obj is GameObject go && go != null)
                        {
                            objects.Add(go);
                            instanceIds.Add(go.GetInstanceID());
                        }
                    }
                }

                if (objects.Count > 0)
                {
                    m_layerObjectCache[layer.guid] = objects;
                    s_layerInstanceIds[layer.guid] = instanceIds;
                }
            }

            m_cacheValid = true;
            s_instanceIdCacheValid = true;

            SaveInstanceIdsToSessionState();
        }

        private static void FastRebuildFromInstanceIds()
        {
            if (!s_instanceIdCacheValid || m_instance?.m_database == null)
            {
                RebuildCacheIfNeeded();
                return;
            }

            m_layerObjectCache.Clear();

            foreach (var layer in m_instance.m_database.layers)
            {
                if (!s_layerInstanceIds.TryGetValue(layer.guid, out var instanceIds))
                    continue;

                var objects = new HashSet<GameObject>();

                foreach (var instanceId in instanceIds)
                {
                    var obj = EditorUtility.InstanceIDToObject(instanceId) as GameObject;
                    if (obj != null)
                    {
                        objects.Add(obj);
                    }
                }

                if (objects.Count > 0)
                {
                    m_layerObjectCache[layer.guid] = objects;
                }
            }

            m_cacheValid = true;
        }

        private void BuildCacheInPlayMode()
        {
            if (m_database?.layers == null) return;

            m_layerObjectCache.Clear();
            s_layerInstanceIds.Clear();

            foreach (var layer in m_database.layers)
            {
                if (layer.objectGlobalIds == null || layer.objectGlobalIds.Count == 0)
                    continue;

                var objects = new HashSet<GameObject>();
                var instanceIds = new HashSet<int>();

                foreach (var goidStr in layer.objectGlobalIds)
                {
                    if (string.IsNullOrEmpty(goidStr)) continue;

                    if (GlobalObjectId.TryParse(goidStr, out var goid))
                    {
                        var obj = GlobalObjectId.GlobalObjectIdentifierToObjectSlow(goid);
                        if (obj is GameObject go && go != null)
                        {
                            objects.Add(go);
                            instanceIds.Add(go.GetInstanceID());
                        }
                    }
                }

                if (objects.Count > 0)
                {
                    m_layerObjectCache[layer.guid] = objects;
                    s_layerInstanceIds[layer.guid] = instanceIds;
                }
            }

            m_cacheValid = true;
            s_instanceIdCacheValid = true;
        }
        private void UpdateLayerObjectCount(string layerGuid)
        {
            RebuildCacheIfNeeded();
            int count = m_layerObjectCache.TryGetValue(layerGuid, out var objects) ? objects.Count : 0;
            m_layerObjectCounts[layerGuid] = count;
        }

        private int GetCachedObjectCount(string layerGuid)
        {
            if (!m_layerObjectCounts.TryGetValue(layerGuid, out var count))
            {
                UpdateLayerObjectCount(layerGuid);
                count = m_layerObjectCounts[layerGuid];
            }
            return count;
        }

        private void OnSelectionChanged()
        {
            RequestRepaint();
        }
        private void OnSceneOpened(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.OpenSceneMode mode)
        {
            EditorApplication.delayCall += () => {
                SwitchToSceneDatabase();
                RequestRepaint();
            };
        }
        private void OnNewSceneCreated(UnityEngine.SceneManagement.Scene scene, UnityEditor.SceneManagement.NewSceneSetup setup, UnityEditor.SceneManagement.NewSceneMode mode)
        {
            EditorApplication.delayCall += () => {
                SwitchToSceneDatabase();
                RequestRepaint();
            };
        }
        private SceneLayerDatabase LoadSceneSpecificDatabase()
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            string scenePath = activeScene.path;

            if (string.IsNullOrEmpty(scenePath))
            {
                return CreateTemporaryDatabase();
            }
            string sceneDir = System.IO.Path.GetDirectoryName(scenePath);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            string dbPath = $"{sceneDir}/{sceneName}_SceneLayers.asset";
            var existingDb = AssetDatabase.LoadAssetAtPath<SceneLayerDatabase>(dbPath);
            if (existingDb != null)
            {
                return existingDb;
            }
            var newDb = ScriptableObject.CreateInstance<SceneLayerDatabase>();
            if (!string.IsNullOrEmpty(sceneDir) && !AssetDatabase.IsValidFolder(sceneDir))
            {
                System.IO.Directory.CreateDirectory(sceneDir);
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(newDb, dbPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            return newDb;
        }
        private SceneLayerDatabase CreateTemporaryDatabase()
        {
            return ScriptableObject.CreateInstance<SceneLayerDatabase>();
        }
        private void SwitchToSceneDatabase()
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            string newScenePath = activeScene.path;
            if (m_currentScenePath == newScenePath) return;

            m_currentScenePath = newScenePath;
            if (m_database != null)
            {
                SaveAllOrders();
                EditorUtility.SetDirty(m_database);
            }

            ClearSessionState();

            m_database = LoadSceneSpecificDatabase();
            LoadAllOrders();
            m_layerFoldouts.Clear();
            m_selectedLayerGuid = null;
            m_searchFilter = "";
            m_hasActiveSearch = false;
            m_matchingLayerGuids.Clear();
            m_matchingObjects.Clear();
            var allObjects = SceneLayerController.EnumerateAllSceneObjects().ToList();
            UpdateObjectTracking(allObjects);

            EditorApplication.RepaintHierarchyWindow();
        }

        private void SaveUIState()
        {
            if (m_database == null) return;
            string sceneKey = GetSceneSpecificKey();
            foreach (var kvp in m_layerFoldouts)
            {
                EditorPrefs.SetBool($"{FOLDOUT_PREF_PREFIX}{sceneKey}.{kvp.Key}", kvp.Value);
            }
            if (!string.IsNullOrEmpty(m_selectedLayerGuid))
            {
                EditorPrefs.SetString($"{SELECTED_LAYER_PREFIX}.{sceneKey}", m_selectedLayerGuid);
            }
            EditorPrefs.SetBool($"{SHOW_VIEW_MANAGER_PREFIX}.{sceneKey}", m_showViewManager);
        }
        private void RestoreUIState()
        {
            if (m_database == null) return;

            string sceneKey = GetSceneSpecificKey();
            m_layerFoldouts.Clear();
            foreach (var layer in m_database.layers)
            {
                string key = $"{FOLDOUT_PREF_PREFIX}{sceneKey}.{layer.guid}";
                if (EditorPrefs.HasKey(key))
                {
                    m_layerFoldouts[layer.guid] = EditorPrefs.GetBool(key);
                }
            }
            string selectedKey = $"{SELECTED_LAYER_PREFIX}.{sceneKey}";
            if (EditorPrefs.HasKey(selectedKey))
            {
                string savedSelection = EditorPrefs.GetString(selectedKey);
                if (m_database.layers.Any(l => l.guid == savedSelection))
                {
                    m_selectedLayerGuid = savedSelection;
                }
            }
            string vmKey = $"{SHOW_VIEW_MANAGER_PREFIX}.{sceneKey}";
            if (EditorPrefs.HasKey(vmKey))
            {
                m_showViewManager = EditorPrefs.GetBool(vmKey);
            }
        }
        private string GetSceneSpecificKey()
        {
            var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(activeScene.path))
            {
                return "TempScene";
            }
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(activeScene.path);
            int pathHash = activeScene.path.GetHashCode();
            return $"{sceneName}_{pathHash}";
        }

        private void SetLayerFoldout(string layerGuid, bool isOpen)
        {
            m_layerFoldouts[layerGuid] = isOpen;
            string sceneKey = GetSceneSpecificKey();
            EditorPrefs.SetBool($"{FOLDOUT_PREF_PREFIX}{sceneKey}.{layerGuid}", isOpen);
        }
        private void SetSelectedLayer(string layerGuid)
        {
            m_selectedLayerGuid = layerGuid;
            string sceneKey = GetSceneSpecificKey();
            EditorPrefs.SetString($"{SELECTED_LAYER_PREFIX}.{sceneKey}", layerGuid ?? "");
        }
        private void SetShowViewManager(bool show)
        {
            m_showViewManager = show;
            string sceneKey = GetSceneSpecificKey();
            EditorPrefs.SetBool($"{SHOW_VIEW_MANAGER_PREFIX}.{sceneKey}", show);
        }

        private void ClearAllLayers()
        {
            if (m_database == null) return;

            Undo.RecordObject(m_database, "Clear All Layers");

            m_database.layers.Clear();
            m_database.layers.Clear();
            if (m_database.views != null) m_database.views.Clear();
            m_layerFoldouts.Clear();
            m_selectedLayerGuid = null;

            EditorUtility.SetDirty(m_database);
            EditorApplication.RepaintHierarchyWindow();
            RequestRepaint();

            ShowNotification(new GUIContent("Cleared all layers for new scene"));
        }
        private void RequestRepaint()
        {
            if (!m_repaintRequested)
            {
                m_repaintRequested = true;
                EditorApplication.delayCall += () =>
                {
                    if (this != null)
                    {
                        Repaint();
                        m_repaintRequested = false;
                    }
                };
            }
        }

        private void OnHierarchyChanged()
        {
            var currentObjects = SceneLayerController.EnumerateAllSceneObjects().ToList();
            int currentObjectCount = currentObjects.Count;
            bool countChanged = currentObjectCount != m_lastKnownObjectCount;
            if (!EditorApplication.isPlayingOrWillChangePlaymode &&
                m_database?.layers != null &&
                m_database.layers.Any(l => l.autoRules?.Count > 0))
            {
                var objectsToCheck = new List<GameObject>();
                if (countChanged && currentObjectCount > m_lastKnownObjectCount)
                {
                    var newObjects = currentObjects.Where(go =>
                        go != null && !m_knownObjectComponents.ContainsKey(go.GetInstanceID())).ToList();
                    objectsToCheck.AddRange(newObjects);
                }
                foreach (var go in currentObjects)
                {
                    if (go == null) continue;
                    int instanceId = go.GetInstanceID();
                    var currentComponents = GetComponentSignature(go);
                    if (m_knownObjectComponents.TryGetValue(instanceId, out var knownComponents))
                    {
                        if (!knownComponents.SetEquals(currentComponents))
                        {
                            var addedComponents = currentComponents.Except(knownComponents).ToList();
                            if (addedComponents.Count > 0)
                            {
                                objectsToCheck.Add(go);
                            }
                        }
                    }
                }
                if (objectsToCheck.Count > 0)
                {
                    m_objectsNeedingAutoAssign = objectsToCheck.Distinct().ToList();
                    m_liveAssignDirty = true;
                    m_liveAssignNextTime = EditorApplication.timeSinceStartup + m_liveAssignDebounceSec;
                }
            }

            if (countChanged)
            {
                InvalidateCache();
            }
            UpdateObjectTracking(currentObjects);
            Repaint();
        }

        private static HashSet<string> GetComponentSignature(GameObject go)
        {
            var signature = new HashSet<string>();
            if (go == null) return signature;

            var components = go.GetComponents<Component>();
            foreach (var component in components)
            {
                if (component != null)
                {
                    signature.Add(component.GetType().FullName);
                }
            }
            return signature;
        }
        private void OnEditorUpdate()
        {
            if (m_liveAssignDirty && EditorApplication.timeSinceStartup >= m_liveAssignNextTime)
            {
                m_liveAssignDirty = false;
                TryRunLiveAutoAssign();
            }
        }

        private void TryRunLiveAutoAssign()
        {
            if (m_database == null || m_database.layers == null || m_objectsNeedingAutoAssign.Count == 0) return;

            bool anyAssigned = false;
            foreach (var layer in m_database.layers)
            {
                if (layer.autoRules == null || layer.autoRules.Count == 0) continue;

                var matchingObjects = GatherObjectsMatchingRulesFromList(layer, m_objectsNeedingAutoAssign);
                if (matchingObjects.Count > 0)
                {
                    var objectsToAssign = matchingObjects.Where(go =>
                    {
                        string goid = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();
                        return layer.objectGlobalIds == null || !layer.objectGlobalIds.Contains(goid);
                    }).ToList();

                    if (objectsToAssign.Count > 0)
                    {
                        AssignObjectsToLayer(objectsToAssign, layer, m_database);
                        anyAssigned = true;
                    }
                }
            }

            m_objectsNeedingAutoAssign.Clear();

            if (anyAssigned)
            {
                EditorApplication.RepaintHierarchyWindow();
                Repaint();
            }
        }
        private void UpdateObjectTracking(List<GameObject> currentObjects)
        {
            m_lastKnownObjectCount = currentObjects.Count;
            m_knownObjectComponents.Clear();

            foreach (var go in currentObjects)
            {
                if (go != null)
                {
                    m_knownObjectComponents[go.GetInstanceID()] = GetComponentSignature(go);
                }
            }
        }

        private void OnLostFocus()
        {
            m_hoverObjectForHierarchy = null;
            m_hoverLayerGuidForHierarchy = null;
            RequestHierarchyRepaintIfNeeded();
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                string sceneName = string.IsNullOrEmpty(activeScene.name) ? "Untitled Scene" : activeScene.name;
                bool isSceneDirty = activeScene.isDirty;
                var sceneNameStyle = new GUIStyle(EditorStyles.miniLabel);
                sceneNameStyle.normal.textColor = EditorGUIUtility.isProSkin
                    ? new Color(0.7f, 0.7f, 0.7f, 1f)
                    : new Color(0.4f, 0.4f, 0.4f, 1f);
                string displayName = isSceneDirty ? $"{sceneName}*" : sceneName;
                GUILayout.Label(displayName, sceneNameStyle);

                GUILayout.FlexibleSpace();
                var presetIcon = EditorGUIUtility.IconContent("Preset.Context");
                var cleanButtonStyle = new GUIStyle(GUIStyle.none);
                cleanButtonStyle.fixedWidth = 24;
                cleanButtonStyle.fixedHeight = 18;

                using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode))
                {
                    if (GUILayout.Button(new GUIContent(presetIcon?.image, "Layer Presets"), cleanButtonStyle))
                    {
                        ShowPresetManager();
                    }
                }
            }

            EditorGUILayout.Space(3);
            var separatorRect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(separatorRect, GetSeparatorColor());

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                EditorGUILayout.Space(2);
                EditorGUILayout.HelpBox(
                    "Layer modifications disabled in Play Mode. View-only mode active.",
                    MessageType.Warning
                );
                EditorGUILayout.Space(2);
            }

            m_hoverLayerGuidInPanel = null;
            if (m_draggingObj && Event.current.type == EventType.MouseDrag)
            {
                var mousePos = Event.current.mousePosition;
                const float scrollZoneHeight = 20f;
                const float scrollSpeed = 300f;

                var windowRect = new Rect(0, 60, position.width, position.height - 60);

                if (mousePos.y < windowRect.y + scrollZoneHeight && m_scrollPosition.y > 0)
                {
                    m_scrollPosition.y = Mathf.Max(0, m_scrollPosition.y - scrollSpeed * Time.unscaledDeltaTime);
                    RequestRepaint();
                }
                else if (mousePos.y > windowRect.yMax - scrollZoneHeight)
                {
                    m_scrollPosition.y += scrollSpeed * Time.unscaledDeltaTime;
                    RequestRepaint();
                }
            }
            if (m_draggingObj && Event.current.type == EventType.MouseDrag)
            {
                RequestRepaint();
            }
            bool anyDragActive = IsAnyObjectDragActive();
            if (!anyDragActive)
            {
                m_extDragActive = false;
                m_extDragTargetLayerGuid = null;
                m_extDragInsertIndex = -1;
            }

            EditorGUILayout.Space();
            DrawTopBar();
            EditorGUILayout.Space();
            if (m_hasActiveSearch)
            {
                int layerCount = m_matchingLayerGuids.Count;
                int objectCount = m_matchingObjects.Count;
                EditorGUILayout.HelpBox($"Showing {layerCount} layer(s) and {objectCount} object(s) matching '{m_searchFilter}'", MessageType.Info);
                EditorGUILayout.Space(2);
            }
            if (Event.current.type == EventType.Repaint &&
                DragAndDrop.objectReferences.Length == 0 &&
                m_dragAssignHoverLayerGuid != null)
            {
                m_dragAssignHoverLayerGuid = null;
            }
            if (!IsAnyObjectDragActive())
            {
                m_hoverExpandLayerGuidForDrag = null;
                m_hoverExpandStartTime = 0.0;
            }

            m_prevHoverObjectForHierarchy = m_hoverObjectForHierarchy;
            m_hoverObjectForHierarchy = null;
            m_hoverLayerGuidForHierarchy = null;

            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);
            DrawLayerList();

            EditorGUILayout.EndScrollView();
            if (m_draggingLayer && (m_pendingDrop || Event.current.type == EventType.MouseUp))
            {
                int target = (m_dragPreviewInsertIndex >= 0) ? m_dragPreviewInsertIndex : 0;
                m_dragInsertIndex = target;
                CommitLayerReorder();

                m_draggingLayer = false;
                m_pendingDrop = false;
                m_dragPotentialIndex = -1;
                m_dragFromIndex = -1;
                m_dragInsertIndex = -1;
                m_dragPreviewInsertIndex = -1;
                m_dragControlId = -1;

                if (Event.current.type == EventType.MouseUp) Event.current.Use();
            }

            RequestHierarchyRepaintIfNeeded();
            if (!string.IsNullOrEmpty(m_deferredExpandLayerGuid))
            {
                m_layerFoldouts[m_deferredExpandLayerGuid] = true;
                m_selectedLayerGuid = m_deferredExpandLayerGuid;
                EditorApplication.RepaintHierarchyWindow();

                if (m_draggingObj)
                {
                    m_dragObjTargetLayerGuid = m_deferredExpandLayerGuid;
                    m_dragObjInsertIndex = 0;
                    m_dragObjPreviewInsertIndex = 0;
                }
                else if (DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
                {
                    m_extDragActive = true;
                    m_extDragTargetLayerGuid = m_deferredExpandLayerGuid;
                    m_extDragInsertIndex = 0;
                }

                m_deferredExpandLayerGuid = null;
                RequestRepaint();
            }
        }
        private static Color GetSeparatorColor()
        {
            return EditorGUIUtility.isProSkin
                ? new Color(1f, 1f, 1f, 0.1f)
                : new Color(0f, 0f, 0f, 0.15f);
        }
        private void ShowPresetManager()
        {
            var mousePos = Event.current.mousePosition;
            PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0),
                new LayerPresetManagerPopup(m_database, this));
        }

        private SceneLayerPreset CreatePresetFromCurrentState(string presetName)
        {
            var preset = ScriptableObject.CreateInstance<SceneLayerPreset>();
            preset.presetName = presetName;
            foreach (var layer in m_database.layers)
            {
                bool isExpanded = m_layerFoldouts.TryGetValue(layer.guid, out var expanded) && expanded;

                var layerData = new SceneLayerPreset.LayerDefinitionData
                {
                    displayName = layer.displayName,
                    color = layer.color,
                    defaultVisible = layer.defaultVisible,
                    defaultPickable = layer.defaultPickable,
                    isExpanded = isExpanded
                };
                if (layer.autoRules != null)
                {
                    foreach (var rule in layer.autoRules)
                    {
                        layerData.autoRules.Add(new SceneLayerPreset.AutoAssignRuleData
                        {
                            componentTypeName = rule.componentTypeName
                        });
                    }
                }

                preset.layers.Add(layerData);
            }
            if (m_database.views != null)
            {
                foreach (var view in m_database.views)
                {
                    var viewData = new SceneLayerPreset.LayerViewData
                    {
                        viewName = view.viewName,
                        color = view.color
                    };

                    if (view.states != null)
                    {
                        foreach (var state in view.states)
                        {
                            var layer = GetLayerByGuid(state.layerGuid);
                            if (layer != null)
                            {
                                viewData.states.Add(new SceneLayerPreset.LayerStateData
                                {
                                    layerDisplayName = layer.displayName,
                                    visible = state.visible,
                                    pickable = state.pickable,
                                    expanded = state.expanded
                                });
                            }
                        }
                    }

                    preset.views.Add(viewData);
                }
            }

            return preset;
        }
        private void ApplyPreset(SceneLayerPreset preset, bool runRuleScan)
        {
            if (preset == null) return;

            Undo.RecordObject(m_database, "Apply Layer Preset");
            m_database.layers.Clear();
            if (m_database.views == null) m_database.views = new List<SceneLayerDatabase.LayerView>();
            m_database.views.Clear();
            m_layerFoldouts.Clear();
            foreach (var layerData in preset.layers)
            {
                var newLayer = new SceneLayerDatabase.LayerDefinition
                {
                    guid = System.Guid.NewGuid().ToString(),
                    displayName = layerData.displayName,
                    color = layerData.color,
                    defaultVisible = layerData.defaultVisible,
                    defaultPickable = layerData.defaultPickable
                };
                if (layerData.autoRules != null && layerData.autoRules.Count > 0)
                {
                    newLayer.autoRules = new List<SceneLayerDatabase.LayerDefinition.AutoAssignRule>();
                    foreach (var ruleData in layerData.autoRules)
                    {
                        newLayer.autoRules.Add(new SceneLayerDatabase.LayerDefinition.AutoAssignRule
                        {
                            componentTypeName = ruleData.componentTypeName
                        });
                    }
                }

                m_database.layers.Add(newLayer);
                m_layerFoldouts[newLayer.guid] = layerData.isExpanded;
            }
            foreach (var viewData in preset.views)
            {
                var newView = new SceneLayerDatabase.LayerView
                {
                    viewName = viewData.viewName,
                    color = viewData.color,
                    states = new List<SceneLayerDatabase.LayerState>()
                };

                if (viewData.states != null)
                {
                    foreach (var stateData in viewData.states)
                    {
                        var matchingLayer = m_database.layers.FirstOrDefault(l => l.displayName == stateData.layerDisplayName);
                        if (matchingLayer != null)
                        {
                            newView.states.Add(new SceneLayerDatabase.LayerState
                            {
                                layerGuid = matchingLayer.guid,
                                visible = stateData.visible,
                                pickable = stateData.pickable,
                                expanded = stateData.expanded
                            });
                        }
                    }
                }

                m_database.views.Add(newView);
            }

            EditorUtility.SetDirty(m_database);
            if (runRuleScan)
            {
                int totalAdded = 0;
                foreach (var layer in m_database.layers)
                {
                    if (layer.autoRules != null && layer.autoRules.Count > 0)
                    {
                        totalAdded += ScanSceneForLayerRules(m_database, layer);
                    }
                }

                if (totalAdded > 0)
                {
                    ShowNotification(new GUIContent($"Applied preset and assigned {totalAdded} objects"));
                }
            }

            EditorApplication.RepaintHierarchyWindow();
            RequestRepaint();
        }
        internal class LayerPresetManagerPopup : PopupWindowContent
        {
            private readonly SceneLayerDatabase _db;
            private readonly SceneLayerManagerWindow _owner;
            private Vector2 _scroll;
            private string _newPresetName = "";
            private bool _runScanOnLoad = true;

            public LayerPresetManagerPopup(SceneLayerDatabase db, SceneLayerManagerWindow owner)
            {
                _db = db;
                _owner = owner;
            }

            public override Vector2 GetWindowSize() => new Vector2(320, 500);

            public override void OnGUI(Rect rect)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Layer Presets", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("✕", GUILayout.Width(20), GUILayout.Height(16)))
                    {
                        editorWindow.Close();
                    }
                }

                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Save Current Setup", EditorStyles.miniBoldLabel);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Label("Name:", GUILayout.Width(50));
                        _newPresetName = EditorGUILayout.TextField(_newPresetName);

                        if (GUILayout.Button("Save", GUILayout.Width(60)))
                        {
                            SaveCurrentAsPreset();
                        }
                    }
                }

                EditorGUILayout.Space();
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Load Options:", EditorStyles.miniBoldLabel);
                    GUILayout.FlexibleSpace();
                    _runScanOnLoad = EditorGUILayout.Toggle("Run rule scan on load", _runScanOnLoad);
                }

                EditorGUILayout.Space();
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Scene Management", EditorStyles.miniBoldLabel);

                    using (new EditorGUILayout.HorizontalScope())
                    {
                        if (GUILayout.Button("Clear All Layers", GUILayout.Height(20)))
                        {
                            if (EditorUtility.DisplayDialog("Clear All Layers?",
                                "This will remove all layers and views from the current scene. Continue?",
                                "Clear", "Cancel"))
                            {
                                _owner.ClearAllLayers();
                                editorWindow.Close();
                            }
                        }
                    }
                }

                EditorGUILayout.Space();
                GUILayout.Label("Available Presets", EditorStyles.miniBoldLabel);

                _scroll = EditorGUILayout.BeginScrollView(_scroll, GUILayout.ExpandHeight(true));

                var presetGuids = AssetDatabase.FindAssets("t:SceneLayerPreset");
                if (presetGuids.Length == 0)
                {
                    EditorGUILayout.HelpBox("No presets found. Save your current setup to create one.", MessageType.Info);
                }
                else
                {
                    foreach (var guid in presetGuids)
                    {
                        var path = AssetDatabase.GUIDToAssetPath(guid);
                        var preset = AssetDatabase.LoadAssetAtPath<SceneLayerPreset>(path);
                        if (preset == null) continue;

                        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                        {
                            var presetIcon = EditorGUIUtility.IconContent("Preset.Context");
                            GUILayout.Label(presetIcon, GUILayout.Width(20));

                            using (new EditorGUILayout.VerticalScope())
                            {
                                GUILayout.Label(preset.presetName, EditorStyles.boldLabel);
                                GUILayout.Label($"{preset.layers.Count} layers, {preset.views.Count} views", EditorStyles.miniLabel);
                            }

                            GUILayout.FlexibleSpace();

                            if (GUILayout.Button("Load", GUILayout.Width(60)))
                            {
                                if (EditorUtility.DisplayDialog("Load Preset?",
                                    $"Loading '{preset.presetName}' will replace all current layers and views. Continue?",
                                    "Load", "Cancel"))
                                {
                                    _owner.ApplyPreset(preset, _runScanOnLoad);
                                    editorWindow.Close();
                                }
                            }

                            if (GUILayout.Button("✕", GUILayout.Width(20)))
                            {
                                if (EditorUtility.DisplayDialog("Delete Preset?",
                                    $"Delete preset '{preset.presetName}'?", "Delete", "Cancel"))
                                {
                                    AssetDatabase.DeleteAsset(path);
                                    AssetDatabase.Refresh();
                                }
                            }
                        }
                    }
                }

                EditorGUILayout.EndScrollView();
            }

            private void SaveCurrentAsPreset()
            {
                if (string.IsNullOrWhiteSpace(_newPresetName))
                {
                    EditorUtility.DisplayDialog("Invalid Name", "Please enter a preset name.", "OK");
                    return;
                }

                var preset = _owner.CreatePresetFromCurrentState(_newPresetName.Trim());
                const string presetDir = "Assets/SceneLayers/Presets";
                if (!AssetDatabase.IsValidFolder(presetDir))
                {
                    AssetDatabase.CreateFolder("Assets/SceneLayers", "Presets");
                }

                string path = $"{presetDir}/{_newPresetName.Trim()}.asset";
                path = AssetDatabase.GenerateUniqueAssetPath(path);

                AssetDatabase.CreateAsset(preset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                _newPresetName = "";
                _owner.ShowNotification(new GUIContent($"Saved preset: {preset.presetName}"));

                editorWindow.Repaint();
            }
        }
        private bool IsAnyObjectDragActive()
        {
            if (m_draggingObj) return true;
            var refs = DragAndDrop.objectReferences;
            return (refs != null && refs.Length > 0 && HasGameObjectsInDrag(refs));
        }

        private bool HasGameObjectsInDrag(UnityEngine.Object[] refs)
        {
            foreach (var r in refs)
            {
                if (r is GameObject go && go.scene.IsValid()) return true;
                if (r is Component c && c && c.gameObject.scene.IsValid()) return true;
            }
            return false;
        }

        private void RequestHierarchyRepaintIfNeeded()
        {
            bool needsRepaint = m_hoverObjectForHierarchy != m_prevHoverObjectForHierarchy;
            if (m_selectedLayerGuid != m_prevSelectedLayerGuid)
                needsRepaint = true;
            if (!string.IsNullOrEmpty(m_hoverLayerGuidInPanel))
                needsRepaint = true;

            if (needsRepaint)
            {
                m_prevSelectedLayerGuid = m_selectedLayerGuid;
                EditorApplication.RepaintHierarchyWindow();
            }
        }

        private static Color GetNeutralHierarchyColor(bool emphasized)
        {
            if (EditorGUIUtility.isProSkin)
                return new Color(0.45f, 0.55f, 0.80f, emphasized ? 0.16f : 0.10f);
            else
                return new Color(0.40f, 0.50f, 0.75f, emphasized ? 0.22f : 0.14f);
        }

        private void DrawTopBar()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Layer Views", EditorStyles.boldLabel);

                    GUILayout.FlexibleSpace();
                    var settingsIcon = EditorGUIUtility.IconContent("Settings");
                    var saveIcon = EditorGUIUtility.IconContent("SaveAs");
                    var cleanButtonStyle = new GUIStyle(GUIStyle.none);
                    cleanButtonStyle.fixedWidth = 24;
                    cleanButtonStyle.fixedHeight = 18;
                    var oldColor = GUI.color;
                    if (m_showViewManager)
                    {
                        GUI.color = new Color(0.6f, 0.8f, 1f, 1f);
                    }

                    bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
                    using (new EditorGUI.DisabledScope(isPlayMode))
                    {
                        if (GUILayout.Button(new GUIContent(settingsIcon?.image, isPlayMode ? "Manage Views (disabled in Play Mode)" : "Manage Views"),
                            cleanButtonStyle))
                        {
                            SetShowViewManager(!m_showViewManager);
                        }
                    }

                    GUI.color = oldColor;

                    using (new EditorGUI.DisabledScope(isPlayMode))
                    {
                        if (GUILayout.Button(new GUIContent(saveIcon?.image, isPlayMode ? "Save Current View (disabled in Play Mode)" : "Save Current View"),
                            cleanButtonStyle))
                        {
                            ShowSaveViewPopup();
                        }
                    }
                }
                if (m_showViewManager)
                {
                    EditorGUILayout.Space(2);
                    DrawSlickViewManager();
                }
                if (m_database.views != null && m_database.views.Count > 0)
                {
                    EditorGUILayout.Space(3);
                    DrawViewButtonsAsChips();
                }
            }

            EditorGUILayout.Space(3);
            var separatorRect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(separatorRect, GetSeparatorColor());

            EditorGUILayout.Space(3);
            GUILayout.Label("Layers", EditorStyles.boldLabel);
            EditorGUILayout.Space(1);
            using (new EditorGUILayout.HorizontalScope())
            {
                var searchIcon = EditorGUIUtility.IconContent("Search Icon");
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label(searchIcon, GUILayout.Width(16), GUILayout.Height(18));

                    EditorGUI.BeginChangeCheck();
                    string newSearch = EditorGUILayout.TextField(m_searchFilter, GUILayout.ExpandWidth(true));
                    if (EditorGUI.EndChangeCheck())
                    {
                        m_searchFilter = newSearch;
                        UpdateSearchResults();
                    }
                }

                var plus = EditorGUIUtility.IconContent("Toolbar Plus");
                bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
                using (new EditorGUI.DisabledScope(isPlayMode))
                {
                    string tooltip = isPlayMode ? "Cannot create layers in Play Mode" : "New Layer";
                    if (GUILayout.Button(new GUIContent(plus?.image, tooltip), GUILayout.Width(28), GUILayout.Height(20)))
                    {
                        if (!isPlayMode)
                        {
                            string suggestedName = GenerateNextLayerName();
                            var mousePos = Event.current.mousePosition;
                            PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 1, 1),
                                new SimpleTextPopup("Create Layer", "Layer Name:", suggestedName, (name, color) =>
                                {
                                    if (!string.IsNullOrWhiteSpace(name))
                                    {
                                        Undo.RecordObject(m_database, "Create Layer");
                                        m_database.layers.Add(new SceneLayerDatabase.LayerDefinition
                                        {
                                            displayName = name,
                                            color = color
                                        });
                                        EditorUtility.SetDirty(m_database);
                                        UpdateSearchResults();
                                    }
                                }));
                        }
                    }
                }
            }

            EditorGUILayout.Space(1);
        }

        private void DrawViewButtonsAsChips()
        {
            if (m_database.views == null || m_database.views.Count == 0) return;

            const float minButtonWidth = 90f;
            const float buttonHeight = 22f;
            const float buttonSpacing = 2f;
            const float sidePadding = 4f;
            float availableWidth = position.width - (sidePadding * 2);

            int maxButtonsPerRow = Mathf.FloorToInt((availableWidth + buttonSpacing) / (minButtonWidth + buttonSpacing));
            maxButtonsPerRow = Mathf.Max(1, maxButtonsPerRow);
            var buttonRows = new List<List<SceneLayerDatabase.LayerView>>();
            var currentRow = new List<SceneLayerDatabase.LayerView>();

            foreach (var view in m_database.views)
            {
                if (currentRow.Count >= maxButtonsPerRow)
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
                var paddedRect = new Rect(
                    rowRect.x + sidePadding,
                    rowRect.y,
                    rowRect.width - (sidePadding * 2),
                    rowRect.height
                );
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
                        var currentEvent = Event.current;
                        if (currentEvent.button == 0)
                        {
                            ApplyLayerView(view);
                        }
                        else if (currentEvent.button == 1)
                        {
                            ShowViewContextMenu(view);
                        }
                    }
                    if (Event.current.type == EventType.ContextClick && buttonRect.Contains(Event.current.mousePosition))
                    {
                        ShowViewContextMenu(view);
                        Event.current.Use();
                    }
                    currentX += buttonWidth;
                }
                if (rowIndex < buttonRows.Count - 1)
                {
                    EditorGUILayout.Space(2);
                }
            }
        }

        private void DrawNewLayerButton()
        {
            EditorGUILayout.Space(4);

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                var buttonStyle = new GUIStyle(GUI.skin.button);
                buttonStyle.fontStyle = FontStyle.Bold;
                buttonStyle.fontSize = 11;

                bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
                using (new EditorGUI.DisabledScope(isPlayMode))
                {
                    if (GUILayout.Button("Add New Layer", buttonStyle, GUILayout.Height(24), GUILayout.Width(180)))
                    {
                        if (!isPlayMode)
                        {
                            string suggestedName = GenerateNextLayerName();
                            var mousePos = Event.current.mousePosition;
                            PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 1, 1),
                                new SimpleTextPopup("Create Layer", "Layer Name:", suggestedName, (name, color) =>
                                {
                                    if (!string.IsNullOrWhiteSpace(name))
                                    {
                                        Undo.RecordObject(m_database, "Create Layer");
                                        m_database.layers.Add(new SceneLayerDatabase.LayerDefinition
                                        {
                                            displayName = name,
                                            color = color
                                        });
                                        EditorUtility.SetDirty(m_database);
                                        UpdateSearchResults();
                                        EditorApplication.delayCall += () =>
                                        {
                                            m_scrollPosition.y = float.MaxValue;
                                            Repaint();
                                        };
                                    }
                                }));
                        }
                    }
                }

                GUILayout.FlexibleSpace();
            }

            EditorGUILayout.Space(4);
        }
        private void ShowViewContextMenu(SceneLayerDatabase.LayerView view)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Apply View"), false, () =>
            {
                ApplyLayerView(view);
            });

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Overwrite View"), false, () =>
            {
                UpdateLayerView(view);
            });

            menu.AddSeparator("");
            menu.AddItem(new GUIContent("Delete View"), false, () =>
            {
                if (EditorUtility.DisplayDialog("Delete View?",
                    $"Delete view '{view.viewName}'?", "Delete", "Cancel"))
                {
                    Undo.RecordObject(m_database, "Delete Layer View");
                    m_database.views.Remove(view);
                    EditorUtility.SetDirty(m_database);
                    RequestRepaint();
                }
            });

            menu.ShowAsContext();
        }

        private void DrawSlickViewManager()
        {
            var managerStyle = new GUIStyle(EditorStyles.helpBox);
            managerStyle.padding = new RectOffset(8, 8, 6, 6);

            using (new EditorGUILayout.VerticalScope(managerStyle))
            {
                if (m_database.views == null || m_database.views.Count == 0)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        var infoIcon = EditorGUIUtility.IconContent("console.infoicon.sml");
                        GUILayout.Label(infoIcon, GUILayout.Width(16));
                        GUILayout.Label("No saved views yet", EditorStyles.miniLabel);
                        GUILayout.FlexibleSpace();
                    }
                }
                else
                {
                    for (int i = m_database.views.Count - 1; i >= 0; i--)
                    {
                        var view = m_database.views[i];
                        using (new EditorGUILayout.HorizontalScope())
                        {
                            DrawViewNameWithRename(view);

                            GUILayout.FlexibleSpace();
                            EditorGUI.BeginChangeCheck();
                            Color newColor = EditorGUILayout.ColorField(GUIContent.none, view.color,
                                false, false, false, GUILayout.Width(30), GUILayout.Height(16));
                            if (EditorGUI.EndChangeCheck())
                            {
                                Undo.RecordObject(m_database, "Change View Color");
                                view.color = newColor;
                                EditorUtility.SetDirty(m_database);
                                RequestRepaint();
                            }

                            GUILayout.Space(4);
                            var refreshIcon = EditorGUIUtility.IconContent("Refresh");
                            var deleteIcon = EditorGUIUtility.IconContent("TreeEditor.Trash");
                            var cleanButtonStyle = new GUIStyle(GUIStyle.none);
                            cleanButtonStyle.fixedWidth = 20;
                            cleanButtonStyle.fixedHeight = 16;

                            if (GUILayout.Button(new GUIContent(refreshIcon?.image, "Overwrite View"),
                                cleanButtonStyle))
                            {
                                UpdateLayerView(view);
                            }

                            if (GUILayout.Button(new GUIContent(deleteIcon?.image, "Delete View"),
                                cleanButtonStyle))
                            {
                                if (EditorUtility.DisplayDialog("Delete View?",
                                    $"Delete view '{view.viewName}'?", "Delete", "Cancel"))
                                {
                                    Undo.RecordObject(m_database, "Delete Layer View");
                                    m_database.views.RemoveAt(i);
                                    EditorUtility.SetDirty(m_database);
                                }
                            }
                        }
                        if (i > 0)
                        {
                            EditorGUILayout.Space(2);
                            var separatorRect = EditorGUILayout.GetControlRect(false, 1);
                            EditorGUI.DrawRect(new Rect(separatorRect.x, separatorRect.y, separatorRect.width, 1),
                                new Color(0.5f, 0.5f, 0.5f, 0.3f));
                            EditorGUILayout.Space(2);
                        }
                    }
                }
            }
        }
        private void DrawViewNameWithRename(SceneLayerDatabase.LayerView view)
        {
            var e = Event.current;
            bool isRenaming = m_renamingViews.Contains(view.viewName);

            if (isRenaming)
            {
                string ctrl = "SL_ViewRename_" + view.viewName;
                if (!m_viewRenameBuffers.ContainsKey(view.viewName))
                    m_viewRenameBuffers[view.viewName] = view.viewName;

                GUI.SetNextControlName(ctrl);

                EditorGUI.BeginChangeCheck();
                string newBuf = EditorGUILayout.TextField(m_viewRenameBuffers[view.viewName], GUILayout.ExpandWidth(true));
                if (EditorGUI.EndChangeCheck())
                {
                    m_viewRenameBuffers[view.viewName] = newBuf;
                }
                if (e.type == EventType.KeyDown)
                {
                    if (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter)
                    {
                        CommitViewRename(view);
                        e.Use();
                        return;
                    }
                    else if (e.keyCode == KeyCode.Escape)
                    {
                        CancelViewRename(view);
                        e.Use();
                        return;
                    }
                }
                if (GUI.GetNameOfFocusedControl() == ctrl && !m_viewRenameTextSelected.Contains(view.viewName))
                {
                    var textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    if (textEditor != null)
                    {
                        textEditor.SelectAll();
                        m_viewRenameTextSelected.Add(view.viewName);
                    }
                }
                if (e.type == EventType.MouseDown && !GUILayoutUtility.GetLastRect().Contains(e.mousePosition))
                {
                    CommitViewRename(view);
                    return;
                }
            }
            else
            {
                var nameRect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true));

                if (e.type == EventType.MouseDown && e.button == 0 && nameRect.Contains(e.mousePosition))
                {
                    if (e.clickCount == 2)
                    {
                        m_renamingViews.Add(view.viewName);
                        m_viewRenameBuffers[view.viewName] = view.viewName;
                        m_viewRenameTextSelected.Remove(view.viewName);

                        var ctrl = "SL_ViewRename_" + view.viewName;
                        EditorApplication.delayCall += () =>
                        {
                            GUI.FocusControl(ctrl);
                            EditorGUI.FocusTextInControl(ctrl);
                        };
                        e.Use();
                        RequestRepaint();
                        return;
                    }
                }
                GUI.Label(nameRect, view.viewName);
            }
        }

        private void CommitViewRename(SceneLayerDatabase.LayerView view)
        {
            if (m_viewRenameBuffers.TryGetValue(view.viewName, out var buf))
            {
                var trimmed = buf?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed != view.viewName)
                {
                    if (m_database.views != null && m_database.views.Any(v => v != view && v.viewName == trimmed))
                    {
                        EditorUtility.DisplayDialog("Duplicate Name",
                            $"A view named '{trimmed}' already exists.", "OK");
                        return;
                    }

                    Undo.RecordObject(m_database, "Rename Layer View");
                    string oldName = view.viewName;
                    view.viewName = trimmed;
                    EditorUtility.SetDirty(m_database);
                    m_renamingViews.Remove(oldName);
                    m_viewRenameBuffers.Remove(oldName);
                    m_viewRenameTextSelected.Remove(oldName);

                    ShowNotification(new GUIContent($"Renamed view to: {trimmed}"));
                }
                else
                {
                    m_renamingViews.Remove(view.viewName);
                    m_viewRenameBuffers.Remove(view.viewName);
                    m_viewRenameTextSelected.Remove(view.viewName);
                }
            }

            GUI.FocusControl(null);
            RequestRepaint();
        }

        private void CancelViewRename(SceneLayerDatabase.LayerView view)
        {
            m_renamingViews.Remove(view.viewName);
            m_viewRenameBuffers.Remove(view.viewName);
            m_viewRenameTextSelected.Remove(view.viewName);
            GUI.FocusControl(null);
            RequestRepaint();
        }
        private void ShowSaveViewPopup()
        {
            var mousePos = Event.current.mousePosition;
            PopupWindow.Show(new Rect(mousePos.x, mousePos.y, 0, 0),
                new SaveViewPopup(m_database, this));
        }
        private void ApplyLayerView(SceneLayerDatabase.LayerView view)
        {
            if (view == null || view.states == null) return;

            Undo.RecordObject(m_database, "Apply Layer View");

            foreach (var state in view.states)
            {
                var layer = GetLayerByGuid(state.layerGuid);
                if (layer == null) continue;
                layer.defaultVisible = state.visible;
                layer.defaultPickable = state.pickable;
                SceneLayerController.SetLayerVisibility(m_database, layer, state.visible);
                SceneLayerController.SetLayerPickable(m_database, layer, state.pickable);
                m_layerFoldouts[state.layerGuid] = state.expanded;
            }

            EditorUtility.SetDirty(m_database);
            EditorApplication.RepaintHierarchyWindow();
            RequestRepaint();

            ShowNotification(new GUIContent($"Applied view: {view.viewName}"));
        }
        private void UpdateLayerView(SceneLayerDatabase.LayerView view)
        {
            if (view == null) return;

            Undo.RecordObject(m_database, "Update Layer View");

            view.states = CaptureCurrentLayerStates();

            EditorUtility.SetDirty(m_database);
            ShowNotification(new GUIContent($"Updated view: {view.viewName}"));
        }
        private List<SceneLayerDatabase.LayerState> CaptureCurrentLayerStates()
        {
            var states = new List<SceneLayerDatabase.LayerState>();

            foreach (var layer in m_database.layers)
            {
                bool isExpanded = m_layerFoldouts.TryGetValue(layer.guid, out var expanded) && expanded;

                states.Add(new SceneLayerDatabase.LayerState
                {
                    layerGuid = layer.guid,
                    visible = layer.defaultVisible,
                    pickable = layer.defaultPickable,
                    expanded = isExpanded
                });
            }

            return states;
        }
        internal class SaveViewPopup : PopupWindowContent
        {
            private readonly SceneLayerDatabase _db;
            private readonly SceneLayerManagerWindow _owner;
            private string _viewName = "";
            private Color _viewColor;

            public SaveViewPopup(SceneLayerDatabase db, SceneLayerManagerWindow owner)
            {
                _db = db;
                _owner = owner;
                _viewName = $"View {(_db.views?.Count ?? 0) + 1}";
                _viewColor = new Color(0.3f, 0.50f, 0.7f, 1f);
            }
            public override Vector2 GetWindowSize() => new Vector2(320, 90);

            public override void OnGUI(Rect rect)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.Label("Save Layer View", EditorStyles.boldLabel);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("✕", GUILayout.Width(20), GUILayout.Height(16)))
                    {
                        editorWindow.Close();
                    }
                }

                EditorGUILayout.Space(4);
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Label("Name:", GUILayout.Width(80));

                    GUI.SetNextControlName("ViewNameField");
                    _viewName = EditorGUILayout.TextField(_viewName, GUILayout.Width(160));

                    GUILayout.Space(5);
                    _viewColor = EditorGUILayout.ColorField(GUIContent.none, _viewColor,
                        false, false, false, GUILayout.Width(50));
                }
                if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() != "ViewNameField")
                {
                    GUI.FocusControl("ViewNameField");
                    EditorApplication.delayCall += () =>
                    {
                        var textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                        if (textEditor != null) textEditor.SelectAll();
                    };
                }
                if (Event.current.type == EventType.KeyDown &&
                    (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
                {
                    SaveAction();
                    Event.current.Use();
                }

                EditorGUILayout.Space(6);
                using (new EditorGUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Save", GUILayout.Width(140), GUILayout.Height(24)))
                    {
                        SaveAction();
                    }
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.Space(2);
            }

            private void SaveAction()
            {
                if (string.IsNullOrWhiteSpace(_viewName))
                {
                    EditorUtility.DisplayDialog("Invalid Name", "Please enter a valid view name.", "OK");
                    return;
                }
                if (_db.views != null && _db.views.Any(v => v.viewName == _viewName.Trim()))
                {
                    if (!EditorUtility.DisplayDialog("Duplicate Name",
                        $"A view named '{_viewName.Trim()}' already exists. Overwrite?", "Overwrite", "Cancel"))
                    {
                        return;
                    }

                    _db.views.RemoveAll(v => v.viewName == _viewName.Trim());
                }

                Undo.RecordObject(_db, "Save Layer View");

                if (_db.views == null) _db.views = new List<SceneLayerDatabase.LayerView>();

                var newView = new SceneLayerDatabase.LayerView
                {
                    viewName = _viewName.Trim(),
                    color = _viewColor,
                    states = _owner.CaptureCurrentLayerStates()
                };

                _db.views.Add(newView);
                EditorUtility.SetDirty(_db);

                _owner.ShowNotification(new GUIContent($"Saved view: {newView.viewName}"));
                _owner.Repaint();

                editorWindow.Close();
            }
        }
        private void UpdateSearchResults()
        {
            m_matchingLayerGuids.Clear();
            m_matchingObjects.Clear();
            m_hasActiveSearch = false;

            if (string.IsNullOrWhiteSpace(m_searchFilter))
            {
                return;
            }

            string searchLower = m_searchFilter.ToLower();
            m_hasActiveSearch = true;
            foreach (var layer in m_database.layers)
            {
                if (layer.displayName.ToLower().Contains(searchLower))
                {
                    m_matchingLayerGuids.Add(layer.guid);
                }
            }
            foreach (var layer in m_database.layers)
            {
                var objects = GetOrderedObjects(layer);
                bool hasMatchingObjects = false;

                foreach (var obj in objects)
                {
                    if (obj && obj.name.ToLower().Contains(searchLower))
                    {
                        m_matchingObjects.Add(obj);
                        hasMatchingObjects = true;
                    }
                }
                if (hasMatchingObjects)
                {
                    m_matchingLayerGuids.Add(layer.guid);
                    m_layerFoldouts[layer.guid] = true;
                }
            }
            foreach (var layer in m_database.layers)
            {
                if (layer.displayName.ToLower().Contains(searchLower))
                {
                    m_layerFoldouts[layer.guid] = true;
                }
            }
        }
        private Vector2 GetDragMousePosition()
        {
            return Event.current.mousePosition;
        }
        private static Rect ToScreenRect(Rect guiRect)
        {
            var min = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.xMin, guiRect.yMin));
            var max = GUIUtility.GUIToScreenPoint(new Vector2(guiRect.xMax, guiRect.yMax));
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }
        private Vector2 GetMouseScreen()
        {
            var gui = m_draggingObj ? m_lastDragMouseGuiPos : Event.current.mousePosition;
            return GUIUtility.GUIToScreenPoint(gui);
        }
        private void MaybeExpandOnHeaderHover(Rect header, SceneLayerDatabase.LayerDefinition layer)
        {
            bool isCollapsed = !m_layerFoldouts.TryGetValue(layer.guid, out var isOpen) || !isOpen;
            if (!isCollapsed || m_draggingLayer) return;

            bool anyDragHappening = IsAnyObjectDragActive();
            if (!anyDragHappening) return;

            Vector2 mouse = Event.current.mousePosition;

            if (header.Contains(mouse))
            {
                double now = EditorApplication.timeSinceStartup;

                if (string.IsNullOrEmpty(m_hoverExpandLayerGuidForDrag) || m_hoverExpandLayerGuidForDrag != layer.guid)
                {
                    m_hoverExpandLayerGuidForDrag = layer.guid;
                    m_hoverExpandStartTime = now;
                }
                else
                {
                    double elapsed = now - m_hoverExpandStartTime;
                    if (elapsed >= m_hoverExpandDelaySec)
                    {
                        m_deferredExpandLayerGuid = layer.guid;
                        m_hoverExpandLayerGuidForDrag = null;
                        m_hoverExpandStartTime = 0.0;
                    }
                }
            }
            else
            {
                if (m_hoverExpandLayerGuidForDrag == layer.guid)
                {
                    m_hoverExpandLayerGuidForDrag = null;
                    m_hoverExpandStartTime = 0.0;
                }
            }
        }

        private void DrawLayerList()
        {
            if (m_database?.layers == null || m_database.layers.Count == 0)
            {
                EditorGUILayout.HelpBox("No layers yet. Click '+ New Layer' to create one.", MessageType.Info);
                DrawNewLayerButton();
                return;
            }

            if (m_hasActiveSearch && m_matchingLayerGuids.Count == 0)
            {
                EditorGUILayout.HelpBox($"No layers or objects match '{m_searchFilter}'", MessageType.Info);
                return;
            }
            const int icon = 22;
            const float rowH = 22f;
            float gap = m_baseGap;
            float colorSquare = Mathf.Round(icon * 0.75f);
            float dividerW = icon;
            float gapAfterCount = colorSquare * 0.5f;
            bool reorderEnabled = !m_hasActiveSearch;

            if (m_headerRects.Length != m_database.layers.Count)
                m_headerRects = new Rect[m_database.layers.Count];

            int visibleLayerIndex = 0;

            if (m_headerRects.Length != m_database.layers.Count)
                m_headerRects = new Rect[m_database.layers.Count];

            for (int i = 0; i < m_database.layers.Count; i++)
            {
                var layer = m_database.layers[i];
                if (m_hasActiveSearch && !m_matchingLayerGuids.Contains(layer.guid))
                    continue;

                using (new EditorGUILayout.VerticalScope(FlatLayerBoxStyle))
                {
                    var header = EditorGUILayout.GetControlRect(false, rowH);
                    m_headerRects[visibleLayerIndex] = header;
                    if (Event.current.type == EventType.Repaint)
                    {
                        var headerRect = new Rect(0, header.y - 2, position.width - 4, header.height + 4);
                        Color tint;
                        if (SceneLayerEditorSettings.UseSceneColors)
                        {
                            tint = ComputeReadableTint(layer.color);
                        }
                        else
                        {
                            tint = EditorGUIUtility.isProSkin
                                ? new Color(1f, 1f, 1f, 0.08f)
                                : new Color(0f, 0f, 0f, 0.06f);
                        }
                        EditorGUI.DrawRect(headerRect, tint);
                    }
                    DrawLayerHeaderRow(header, layer, icon, gap, colorSquare, dividerW, gapAfterCount, visibleLayerIndex, reorderEnabled);

                    bool isOpen = m_layerFoldouts.TryGetValue(layer.guid, out var open) && open;
                    float childrenStartY = 0;

                    if (isOpen)
                    {
                        var sep = EditorGUILayout.GetControlRect(false, 2f);
                        childrenStartY = sep.yMax;

                        var line = new Rect(sep.x, sep.y + 1f, sep.width, 1f);
                        EditorGUI.DrawRect(line, GetSeparatorColorDark());
                        HandleSeparatorDragAssign(sep, layer);
                        if (SceneLayerEditorSettings.UseSceneColors && Event.current.type == EventType.Repaint)
                        {
                            float cachedHeight = m_layerChildrenHeights.TryGetValue(layer.guid, out var h) ? h : rowH + 4;

                            var childrenRect = new Rect(0, childrenStartY, position.width - 4, cachedHeight);
                            var lightTint = layer.color;
                            lightTint.a = EditorGUIUtility.isProSkin ? 0.12f : 0.18f;

                            EditorGUI.DrawRect(childrenRect, lightTint);
                        }
                    }
                    else
                    {
                        EditorGUILayout.Space(2);
                    }

                    if (isOpen)
                    {
                        m_childRowRects[layer.guid] = new List<Rect>();
                        var list = GetOrderedObjects(layer);
                        if (m_hasActiveSearch)
                        {
                            list = list.Where(obj => m_matchingObjects.Contains(obj)).ToList();
                        }

                        if (list.Count == 0)
                        {
                            using (new EditorGUILayout.HorizontalScope())
                            {
                                GUILayout.Space(m_foldW + gap);
                                var emptyRect = EditorGUILayout.GetControlRect(false, rowH);
                                string message = m_hasActiveSearch ? $"(No objects match '{m_searchFilter}')" : "(No objects)";
                                EditorGUI.LabelField(emptyRect, message, EditorStyles.miniLabel);
                                if (!m_hasActiveSearch)
                                {
                                    HandleEmptyLayerDragAssign(emptyRect, layer);
                                }
                            }
                        }
                        else
                        {
                            for (int j = 0; j < list.Count; j++)
                                DrawChildRow(layer, list[j], icon, gap, colorSquare, dividerW, rowH, gapAfterCount, j, list.Count);
                            HandleExternalAssignOverChildren(layer);
                        }
                        if (Event.current.type == EventType.Repaint)
                        {
                            var endRect = GUILayoutUtility.GetLastRect();
                            float actualHeight = endRect.yMax - childrenStartY;
                            m_layerChildrenHeights[layer.guid] = actualHeight;
                        }
                    }
                }
                EditorGUILayout.Space(2);

                visibleLayerIndex++;
            }
            if (m_draggingLayer && reorderEnabled && Event.current.type == EventType.Repaint)
            {
                UpdateDragInsertIndexPreciseScreen(Event.current.mousePosition);
                m_dragPreviewInsertIndex = m_dragInsertIndex;
            }

            int guideIndex = (m_dragPreviewInsertIndex >= 0) ? m_dragPreviewInsertIndex : m_dragInsertIndex;
            if (m_draggingLayer && reorderEnabled && m_database.layers.Count > 0 && guideIndex >= 0)
            {
                int count = m_database.layers.Count;
                float yLine;
                if (guideIndex <= 0)
                    yLine = m_headerRects[0].yMin;
                else if (guideIndex >= count)
                    yLine = m_headerRects[count - 1].yMax;
                else
                    yLine = m_headerRects[guideIndex].yMin;

                var w = position.width - 24f;
                var lineRect = new Rect(12f, yLine - 1f, w, 2f);
                var col = EditorGUIUtility.isProSkin ? new Color(0.35f, 0.55f, 1f, 0.9f) : new Color(0.2f, 0.45f, 1f, 0.9f);
                EditorGUI.DrawRect(lineRect, col);
            }
            if (m_dragObj != null && Event.current.type == EventType.Repaint)
            {
                UpdateObjectDragPreview(Event.current.mousePosition);
                m_dragObjPreviewInsertIndex = m_dragObjInsertIndex;
            }

            if (m_draggingObj && !string.IsNullOrEmpty(m_dragObjTargetLayerGuid) && m_dragObjPreviewInsertIndex >= 0)
            {
                float yLine = 0f;
                if (m_childRowRects.TryGetValue(m_dragObjTargetLayerGuid, out var rows) && rows.Count > 0)
                {
                    if (m_dragObjPreviewInsertIndex <= 0) yLine = rows[0].yMin;
                    else if (m_dragObjPreviewInsertIndex >= rows.Count) yLine = rows[rows.Count - 1].yMax;
                    else yLine = rows[m_dragObjPreviewInsertIndex].yMin;
                }
                else
                {
                    int li = m_database.layers.FindIndex(l => l.guid == m_dragObjTargetLayerGuid);
                    var hdr = (li >= 0 && li < m_headerRects.Length) ? m_headerRects[li] : Rect.zero;
                    yLine = hdr.yMax;
                }
                var rGuide = new Rect(12f, yLine - 1f, position.width - 24f, 2f);
                var colObj = EditorGUIUtility.isProSkin ? new Color(0.35f, 0.55f, 1f, 0.9f) : new Color(0.2f, 0.45f, 1f, 0.9f);
                EditorGUI.DrawRect(rGuide, colObj);
            }
            if (((!m_draggingObj && m_extDragActive) || (m_draggingObj && m_extDragActive)) &&
        !string.IsNullOrEmpty(m_extDragTargetLayerGuid) && m_extDragInsertIndex >= 0)
            {
                float yLine = 0f;
                int targetLayerIndex = m_database.layers.FindIndex(l => l.guid == m_extDragTargetLayerGuid);
                bool isTargetLayerExpanded = m_layerFoldouts.TryGetValue(m_extDragTargetLayerGuid, out var expanded) && expanded;

                if (isTargetLayerExpanded && m_childRowRects.TryGetValue(m_extDragTargetLayerGuid, out var rows) && rows.Count > 0)
                {
                    if (m_extDragInsertIndex <= 0)
                        yLine = rows[0].yMin;
                    else if (m_extDragInsertIndex >= rows.Count)
                        yLine = rows[rows.Count - 1].yMax;
                    else
                        yLine = rows[m_extDragInsertIndex].yMin;
                }
                else
                {
                    if (targetLayerIndex >= 0 && targetLayerIndex < m_headerRects.Length)
                    {
                        var headerRect = m_headerRects[targetLayerIndex];
                        yLine = headerRect.yMax + 4f;
                    }
                    else
                    {
                        return;
                    }
                }

                var rGuide = new Rect(12f, yLine - 1f, position.width - 24f, 2f);
                var colObj = EditorGUIUtility.isProSkin ? new Color(0.35f, 0.55f, 1f, 0.9f) : new Color(0.2f, 0.45f, 1f, 0.9f);
                EditorGUI.DrawRect(rGuide, colObj);
            }
            if (m_draggingObj && (m_dragObjPendingDrop || Event.current.type == EventType.MouseUp))
            {
                CommitObjectDrag();

                m_draggingObj = false;
                m_dragObjPendingDrop = false;
                m_dragObj = null;
                m_dragObjSrcLayerGuid = m_dragObjTargetLayerGuid = null;
                m_dragObjFromIndex = m_dragObjInsertIndex = m_dragObjPreviewInsertIndex = -1;

                if (Event.current.type == EventType.MouseUp) Event.current.Use();
            }

            if (!m_hasActiveSearch)
            {
                DrawNewLayerButton();
            }

        }

        private static GUIStyle _brightFoldout;
        private static GUIStyle BrightFoldout
        {
            get
            {
                if (_brightFoldout == null)
                {
                    _brightFoldout = new GUIStyle(EditorStyles.foldout);
                    var bright = EditorGUIUtility.isProSkin ? new Color(1f, 1f, 1f, 0.9f) : new Color(0f, 0f, 0f, 0.9f);
                    _brightFoldout.normal.textColor = bright;
                    _brightFoldout.onNormal.textColor = bright;
                    _brightFoldout.active.textColor = bright;
                    _brightFoldout.onActive.textColor = bright;
                    _brightFoldout.focused.textColor = bright;
                    _brightFoldout.onFocused.textColor = bright;
                }
                return _brightFoldout;
            }
        }

        private void DrawLayerHeaderRow(
            Rect r,
            SceneLayerDatabase.LayerDefinition layer,
            int iconW,
            float gap,
            float colorSquare,
            float dividerW,
            float gapAfterCount,
            int index,
            bool reorderEnabled)
        {
            float x = r.x;
            float y = r.y;
            float h = r.height;
            var e = Event.current;
            if (e.type == EventType.ContextClick && r.Contains(e.mousePosition))
            {
                ShowLayerContextMenu(layer);
                e.Use();
                return;
            }
            if (e.type == EventType.MouseDown && e.button == 1 && r.Contains(e.mousePosition))
            {
                e.Use();
                return;
            }
            var currentEventType = Event.current.type;
            if (currentEventType == EventType.MouseMove ||
                currentEventType == EventType.MouseDrag ||
                currentEventType == EventType.Repaint ||
                currentEventType == EventType.DragUpdated)
            {
                MaybeExpandOnHeaderHover(r, layer);
            }
            bool isOpen = m_layerFoldouts.TryGetValue(layer.guid, out var open) && open;
            var foldRect = new Rect(x, y, m_foldW, h);
            var oldCol = GUI.color;
            GUI.color = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            bool newOpen = EditorGUI.Foldout(foldRect, isOpen, GUIContent.none, true, BrightFoldout);
            GUI.color = oldCol;
            if (newOpen != isOpen) SetLayerFoldout(layer.guid, newOpen);
            else if (!m_layerFoldouts.ContainsKey(layer.guid)) SetLayerFoldout(layer.guid, false);
            x += m_foldW + gap;
            var eyeRect = new Rect(x, y, iconW, h);

            bool shiftHeld = Event.current.shift;

            if (Event.current.type == EventType.KeyDown || Event.current.type == EventType.KeyUp)
            {
                if (Event.current.keyCode == KeyCode.LeftShift || Event.current.keyCode == KeyCode.RightShift)
                {
                    RequestRepaint();
                }
            }

            string eyeTooltip;
            if (shiftHeld)
            {
                eyeTooltip = "Isolate Layer (hide everything else)";
            }
            else
            {
                eyeTooltip = layer.defaultVisible ? "Hide Layer" : "Show Layer";
            }

            var eyeIcon = EditorGUIUtility.IconContent(
                layer.defaultVisible ? "animationvisibilitytoggleon" : "animationvisibilitytoggleoff");

            var oldIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));
            var oldColor = GUI.color;
            GUI.color = EditorGUIUtility.isProSkin ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1f);

            if (GUI.Button(eyeRect, new GUIContent(eyeIcon?.image, eyeTooltip), IconOnlyCenterStyle))
            {
                bool clickedWithShift = Event.current.shift;

                if (clickedWithShift)
                {
                    IsolateLayerHideAll(layer);
                }
                else
                {
                    layer.defaultVisible = !layer.defaultVisible;
                    SceneLayerController.SetLayerVisibility(m_database, layer, layer.defaultVisible);
                    EditorUtility.SetDirty(m_database);
                }
                RequestRepaint();
            }

            GUI.color = oldColor;
            EditorGUIUtility.SetIconSize(oldIconSize);

            x += iconW + gap;

            var lockRect = new Rect(x, y, iconW, h);

            string lockTooltip = layer.defaultPickable
                ? "Lock Layer (disable picking)"
                : "Unlock Layer (enable picking)";

            var lockIcon = EditorGUIUtility.IconContent(
                layer.defaultPickable ? "IN LockButton" : "IN LockButton on");

            var oldIconSize2 = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));

            var oldColor2 = GUI.color;
            GUI.color = EditorGUIUtility.isProSkin ? Color.white : new Color(0.2f, 0.2f, 0.2f, 1f);

            if (GUI.Button(lockRect, new GUIContent(lockIcon?.image, lockTooltip), IconOnlyCenterStyle))
            {
                layer.defaultPickable = !layer.defaultPickable;
                SceneLayerController.SetLayerPickable(m_database, layer, layer.defaultPickable);
                EditorUtility.SetDirty(m_database);
                RequestRepaint();
            }

            GUI.color = oldColor2;
            EditorGUIUtility.SetIconSize(oldIconSize2);

            x += iconW + gap;
            float minNameWidth = 80f;
            float totalWidth = r.width;
            float usedWidth = x - r.x;
            float rightIconsWidth = (iconW + gap) * 3;

            float nameWidth = Mathf.Max(minNameWidth, totalWidth - usedWidth - rightIconsWidth);
            var nameRect = new Rect(x, y, nameWidth, h);

            int id = GUIUtility.GetControlID("SL_LayerDrag".GetHashCode(), FocusType.Passive, nameRect);

            bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
            if (!m_draggingObj && !m_renamingLayers.Contains(layer.guid) && reorderEnabled && !isPlayMode)
            {
                switch (e.GetTypeForControl(id))
                {
                    case EventType.MouseDown:
                        if (e.button == 0 && nameRect.Contains(e.mousePosition) && e.clickCount == 1)
                        {
                            m_dragControlId = id;
                            m_dragPotentialIndex = index;
                            m_dragMouseDownPos = e.mousePosition;
                        }
                        break;

                    case EventType.MouseDrag:
                        if (m_dragControlId == id && GUIUtility.hotControl == 0)
                        {
                            if (Vector2.Distance(e.mousePosition, m_dragMouseDownPos) > 8f)
                            {
                                GUIUtility.hotControl = id;
                                m_draggingLayer = true;
                                m_dragFromIndex = index;
                                m_dragInsertIndex = index;
                                m_dragPreviewInsertIndex = index;
                                e.Use();
                                RequestRepaint();
                            }
                        }
                        else if (m_draggingLayer && GUIUtility.hotControl == id)
                        {
                            e.Use();
                            RequestRepaint();
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == id)
                        {
                            GUIUtility.hotControl = 0;
                            if (m_draggingLayer)
                            {
                                m_pendingDrop = true;
                                e.Use();
                            }
                        }
                        if (m_dragControlId == id)
                        {
                            m_dragControlId = -1;
                            m_dragPotentialIndex = -1;
                        }
                        break;
                }
            }
            HandleNameClicksAndDraw(nameRect, layer);
            float xr = nameRect.xMax + gap;
            IconButtonAbs(new Rect(xr, y, iconW, h), "d_RectTool On", "Select All", "★", () =>
            {
                var objs = SceneLayerController.GetObjectsInLayer(layer.guid, m_database).Where(o => o).Cast<UnityEngine.Object>().ToArray();
                Selection.objects = objs;
                if (objs.Length > 0) EditorGUIUtility.PingObject(objs[0]);
            });
            xr += iconW + gap;

            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                string rulesTooltip = isPlayMode ? "Layer Rules (disabled in Play Mode)" : "Layer Rules…";
                IconButtonAbs(new Rect(xr, y, iconW, h),
                    "SettingsIcon", rulesTooltip, "⚙",
                    () =>
                    {
                        if (isPlayMode) return;
                        var popupRect = new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 0, 0);
                        PopupWindow.Show(popupRect, new AutoAssignRulesPopup(m_database, layer, this));
                    });
            }
            xr += iconW + gap;
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                string deleteTooltip = isPlayMode ? "Delete Layer (disabled in Play Mode)" : "Delete Layer";
                IconButtonAbs(new Rect(xr, y, iconW, h), "TreeEditor.Trash", deleteTooltip, "✕", () =>
                {
                    if (isPlayMode) return;

                    if (EditorUtility.DisplayDialog("Delete Layer?",
                        $"Delete layer '{layer.displayName}'? This will not remove the membership component from objects.",
                        "Delete", "Cancel"))
                    {
                        Undo.RecordObject(m_database, "Delete Layer");
                        m_database.layers.Remove(layer);
                        EditorUtility.SetDirty(m_database);

                        if (m_selectedLayerGuid == layer.guid)
                        {
                            SetSelectedLayer(null);
                            EditorApplication.RepaintHierarchyWindow();
                        }

                        m_layerFoldouts.Remove(layer.guid);
                        if (m_dragAssignHoverLayerGuid == layer.guid)
                            m_dragAssignHoverLayerGuid = null;
                        if (m_hoverExpandLayerGuidForDrag == layer.guid)
                            m_hoverExpandLayerGuidForDrag = null;
                        if (m_deferredExpandLayerGuid == layer.guid)
                            m_deferredExpandLayerGuid = null;

                        GUIUtility.ExitGUI();
                    }
                });
            }
            bool isDropHover = HandleAssignDrag(r, layer);

            if (isDropHover && m_dragAssignHoverLayerGuid == layer.guid && Event.current.type == EventType.Repaint)
            {
                var hl = EditorGUIUtility.isProSkin
                    ? new Color(0.35f, 0.60f, 1f, 0.18f)
                    : new Color(0.20f, 0.50f, 1f, 0.20f);

                EditorGUI.DrawRect(r, hl);
                var b = new Color(hl.r, hl.g, hl.b, hl.a * 1.7f);
                EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, 2f), b);
                EditorGUI.DrawRect(new Rect(r.x, r.yMax - 2f, r.width, 2f), b);
                EditorGUI.DrawRect(new Rect(r.x, r.y, 2f, r.height), b);
                EditorGUI.DrawRect(new Rect(r.xMax - 2f, r.y, 2f, r.height), b);
            }
        }
        private void ShowLayerContextMenu(SceneLayerDatabase.LayerDefinition layer)
        {
            Vector2 mousePosition = Event.current.mousePosition;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Focus Layer (hide other layers)"), false, () =>
            {
                FocusLayerHideOthers(layer);
            });

            menu.AddItem(new GUIContent("Isolate Layer (hide everything else)"), false, () =>
            {
                IsolateLayerHideAll(layer);
            });

            menu.AddSeparator("");
            bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;

            if (isPlayMode)
            {
                menu.AddDisabledItem(new GUIContent("Layer Rules… (disabled in Play Mode)"));
            }
            else
            {
                menu.AddItem(new GUIContent("Layer Rules…"), false, () =>
                {
                    var popupRect = new Rect(mousePosition.x, mousePosition.y, 0, 0);
                    PopupWindow.Show(popupRect, new AutoAssignRulesPopup(m_database, layer, this));
                });
            }
            bool hasRules = layer.autoRules != null && layer.autoRules.Count > 0;
            if (hasRules && !isPlayMode)
            {
                menu.AddItem(new GUIContent("Scan Scene Now"), false, () =>
                {
                    var added = ScanSceneForLayerRules(m_database, layer);
                    if (added > 0)
                    {
                        SetLayerFoldout(layer.guid, true);
                        ShowNotification(new GUIContent($"Added {added} objects to \"{layer.displayName}\""));
                    }
                    else
                    {
                        ShowNotification(new GUIContent("No matching objects found"));
                    }
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent(isPlayMode ? "Scan Scene Now (disabled in Play Mode)" : "Scan Scene Now"));
            }

            menu.AddSeparator("");

            int objectCount = GetCachedObjectCount(layer.guid);
            if (objectCount > 0)
            {
                menu.AddItem(new GUIContent($"Select All Objects ({objectCount})"), false, () =>
                {
                    var objs = SceneLayerController.GetObjectsInLayer(layer.guid, m_database).Where(o => o).Cast<UnityEngine.Object>().ToArray();
                    Selection.objects = objs;
                    if (objs.Length > 0) EditorGUIUtility.PingObject(objs[0]);
                });
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Select All Objects (0)"));
            }

            menu.AddSeparator("");
            var selectedObjects = Selection.gameObjects.Where(go => go.scene.IsValid()).ToArray();

            if (selectedObjects.Length > 0 && !isPlayMode)
            {
                menu.AddItem(new GUIContent($"Assign Selection ({selectedObjects.Length} objects)"), false, () =>
                {
                    var (added, already) = AssignObjectsToLayerWithCounts(selectedObjects, layer, m_database);
                    if (added > 0)
                    {
                        ShowNotification(new GUIContent($"Assigned {added} to \"{layer.displayName}\""));
                        SetLayerFoldout(layer.guid, true);
                    }
                    if (already > 0 && added == 0)
                    {
                        ShowNotification(new GUIContent("Objects already in layer"));
                    }
                });
            }
            else
            {
                string disabledText = isPlayMode ? "Assign Selection (disabled in Play Mode)" : "Assign Selection";
                menu.AddDisabledItem(new GUIContent(disabledText));
            }

            if (selectedObjects.Length > 0 && !isPlayMode)
            {
                menu.AddItem(new GUIContent($"Remove Selection ({selectedObjects.Length} objects)"), false, () =>
                {
                    Undo.RecordObject(m_database, "Remove From Layer");
                    int removed = 0;

                    foreach (var go in selectedObjects)
                    {
                        string goid = GOID(go);
                        if (layer.objectGlobalIds != null && layer.objectGlobalIds.Contains(goid))
                        {
                            layer.objectGlobalIds.Remove(goid);
                            RemoveFromOrder(layer.guid, goid);
                            UpdateCacheForObject(go, removeFromLayerGuid: layer.guid);
                            removed++;
                        }
                    }

                    EditorUtility.SetDirty(m_database);

                    if (removed > 0)
                    {
                        ShowNotification(new GUIContent($"Removed {removed} from \"{layer.displayName}\""));
                    }
                });
            }
            else
            {
                string disabledText = isPlayMode ? "Remove Selection (disabled in Play Mode)" : "Remove Selection";
                menu.AddDisabledItem(new GUIContent(disabledText));
            }

            if (isPlayMode)
            {
                menu.AddDisabledItem(new GUIContent("Rename Layer (disabled in Play Mode)"));
                menu.AddDisabledItem(new GUIContent("Duplicate Layer (disabled in Play Mode)"));
                menu.AddDisabledItem(new GUIContent("Delete Layer (disabled in Play Mode)"));
            }
            else
            {
                menu.AddItem(new GUIContent("Rename Layer"), false, () =>
                {
                    m_renamingLayers.Add(layer.guid);
                    m_renameBuffers[layer.guid] = layer.displayName;
                    m_renameTextSelected.Remove(layer.guid);
                    SetSelectedLayer(layer.guid);
                    EditorApplication.RepaintHierarchyWindow();

                    var ctrl = "SL_Rename_" + layer.guid;
                    EditorApplication.delayCall += () =>
                    {
                        GUI.FocusControl(ctrl);
                        EditorGUI.FocusTextInControl(ctrl);
                    };
                    RequestRepaint();
                });
                if (objectCount > 0)
                {
                    menu.AddItem(new GUIContent($"Duplicate Layer (structure only)"), false, () =>
                    {
                        DuplicateLayer(layer, false);
                    });

                    menu.AddItem(new GUIContent($"Duplicate Layer + Objects ({objectCount})"), false, () =>
                    {
                        DuplicateLayer(layer, true);
                    });
                }
                else
                {
                    menu.AddItem(new GUIContent("Duplicate Layer"), false, () =>
                    {
                        DuplicateLayer(layer, false);
                    });
                }

                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Delete Layer"), false, () =>
                {
                    if (EditorUtility.DisplayDialog("Delete Layer?",
                        $"Delete layer '{layer.displayName}'? Scene objects will remain untouched.",
                        "Delete", "Cancel"))
                    {
                        Undo.RecordObject(m_database, "Delete Layer");
                        m_database.layers.Remove(layer);
                        EditorUtility.SetDirty(m_database);

                        if (m_selectedLayerGuid == layer.guid)
                        {
                            SetSelectedLayer(null);
                            EditorApplication.RepaintHierarchyWindow();
                        }

                        m_layerFoldouts.Remove(layer.guid);
                        if (m_dragAssignHoverLayerGuid == layer.guid)
                            m_dragAssignHoverLayerGuid = null;
                        if (m_hoverExpandLayerGuidForDrag == layer.guid)
                            m_hoverExpandLayerGuidForDrag = null;
                        if (m_deferredExpandLayerGuid == layer.guid)
                            m_deferredExpandLayerGuid = null;

                        EditorApplication.delayCall += () =>
                        {
                            if (this != null) Repaint();
                        };
                    }
                });
            }

            menu.ShowAsContext();
        }

        private void FocusLayerHideOthers(SceneLayerDatabase.LayerDefinition targetLayer)
        {
            if (targetLayer == null) return;

            Undo.RecordObject(m_database, "Focus Layer");
            foreach (var layer in m_database.layers)
            {
                if (layer.guid != targetLayer.guid)
                {
                    layer.defaultVisible = false;
                    SceneLayerController.SetLayerVisibility(m_database, layer, false);
                }
            }
            targetLayer.defaultVisible = true;
            SceneLayerController.SetLayerVisibility(m_database, targetLayer, true);
            SetLayerFoldout(targetLayer.guid, true);

            EditorUtility.SetDirty(m_database);
            EditorApplication.RepaintHierarchyWindow();
            Repaint();

            ShowNotification(new GUIContent($"Focused: {targetLayer.displayName}"));
        }

        private void IsolateLayerHideAll(SceneLayerDatabase.LayerDefinition targetLayer)
        {
            if (targetLayer == null) return;

            Undo.RecordObject(m_database, "Isolate Layer");

            var svm = SceneVisibilityManager.instance;
            var allSceneObjects = SceneLayerController.EnumerateAllSceneObjects()
                .Where(go => go != null)
                .ToArray();
            if (allSceneObjects.Length > 0)
            {
                svm.Hide(allSceneObjects, true);
            }
            RebuildCacheIfNeeded();
            if (m_layerObjectCache.TryGetValue(targetLayer.guid, out var targetObjects))
            {
                var objectsToShow = targetObjects.Where(go => go != null).ToArray();
                if (objectsToShow.Length > 0)
                {
                    svm.Show(objectsToShow, true);
                }
            }
            foreach (var layer in m_database.layers)
            {
                if (layer.guid == targetLayer.guid)
                {
                    layer.defaultVisible = true;
                }
                else
                {
                    layer.defaultVisible = false;
                }
            }
            SetLayerFoldout(targetLayer.guid, true);

            EditorUtility.SetDirty(m_database);
            EditorApplication.RepaintHierarchyWindow();
            m_visibilityCache.Clear();

            Repaint();

            ShowNotification(new GUIContent($"Isolated: {targetLayer.displayName} ({targetObjects?.Count ?? 0} objects)"));
        }

        private void DuplicateLayer(SceneLayerDatabase.LayerDefinition sourceLayer, bool copyObjects)
        {
            Undo.RecordObject(m_database, "Duplicate Layer");

            var newLayer = new SceneLayerDatabase.LayerDefinition
            {
                displayName = sourceLayer.displayName + " Copy",
                color = sourceLayer.color,
                defaultVisible = sourceLayer.defaultVisible,
                defaultPickable = sourceLayer.defaultPickable
            };
            if (sourceLayer.autoRules != null)
            {
                newLayer.autoRules = new List<SceneLayerDatabase.LayerDefinition.AutoAssignRule>();
                foreach (var rule in sourceLayer.autoRules)
                {
                    newLayer.autoRules.Add(new SceneLayerDatabase.LayerDefinition.AutoAssignRule
                    {
                        componentTypeName = rule.componentTypeName
                    });
                }
            }
            if (copyObjects && sourceLayer.objectGlobalIds != null)
            {
                newLayer.objectGlobalIds = new List<string>(sourceLayer.objectGlobalIds);

                if (newLayer.objectGlobalIds.Count > 0)
                {
                    ShowNotification(new GUIContent($"Duplicated layer with {newLayer.objectGlobalIds.Count} objects"));
                    SetLayerFoldout(newLayer.guid, true);
                }
            }

            m_database.layers.Add(newLayer);
            EditorUtility.SetDirty(m_database);
            SetSelectedLayer(newLayer.guid);
            InvalidateCache();
            RebuildCacheIfNeeded();

            Repaint();
        }

        private static void UpdateCacheForObject(GameObject go, string addToLayerGuid = null, string removeFromLayerGuid = null)
        {
            if (!m_cacheValid) return;
            if (!string.IsNullOrEmpty(removeFromLayerGuid) && m_layerObjectCache.TryGetValue(removeFromLayerGuid, out var oldSet))
            {
                oldSet.Remove(go);
                if (m_instance != null)
                {
                    m_instance.m_orderedObjectsCache.Remove(removeFromLayerGuid);
                    m_instance.m_layerObjectCounts.Remove(removeFromLayerGuid);
                }
            }
            if (!string.IsNullOrEmpty(addToLayerGuid))
            {
                if (!m_layerObjectCache.ContainsKey(addToLayerGuid))
                    m_layerObjectCache[addToLayerGuid] = new HashSet<GameObject>();
                m_layerObjectCache[addToLayerGuid].Add(go);
                if (m_instance != null)
                {
                    m_instance.m_orderedObjectsCache.Remove(addToLayerGuid);
                    m_instance.m_layerObjectCounts.Remove(addToLayerGuid);
                }
            }
        }
        public static void ClearVisibilityCacheForObjects(IEnumerable<GameObject> objects)
        {
            if (m_instance == null) return;
            if (objects is GameObject[] objArray)
            {
                for (int i = 0; i < objArray.Length; i++)
                {
                    var go = objArray[i];
                    if (go != null)
                    {
                        m_instance.m_visibilityCache.Remove(go.GetInstanceID());
                    }
                }
            }
            else
            {
                foreach (var go in objects)
                {
                    if (go != null)
                    {
                        m_instance.m_visibilityCache.Remove(go.GetInstanceID());
                    }
                }
            }

            m_instance.Repaint();
        }

        private void ShowObjectContextMenu(GameObject obj, SceneLayerDatabase.LayerDefinition layer)
        {
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Select"), false, () =>
            {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject(obj);
            });
            menu.AddItem(new GUIContent("Ping in Hierarchy"), false, () =>
            {
                EditorGUIUtility.PingObject(obj);
            });

            menu.AddSeparator("");
            menu.AddItem(new GUIContent($"Remove from \"{layer.displayName}\""), false, () =>
            {
                string goid = GOID(obj);
                if (layer.objectGlobalIds != null && layer.objectGlobalIds.Contains(goid))
                {
                    Undo.RecordObject(m_database, "Remove From Layer");
                    layer.objectGlobalIds.Remove(goid);
                    EditorUtility.SetDirty(m_database);
                    RemoveFromOrder(layer.guid, goid);
                    UpdateCacheForObject(obj, removeFromLayerGuid: layer.guid);

                    ShowNotification(new GUIContent($"Removed \"{obj.name}\" from \"{layer.displayName}\""));
                }
            });
            var layerGuids = SceneLayerController.GetLayersForObject(obj, m_database);
            if (layerGuids != null && layerGuids.Count > 1)
            {
                menu.AddSeparator("");
                menu.AddDisabledItem(new GUIContent("Also in layers:"));

                foreach (var guid in layerGuids)
                {
                    if (guid == layer.guid) continue;
                    var otherLayer = GetLayerByGuid(guid);
                    if (otherLayer != null)
                    {
                        menu.AddDisabledItem(new GUIContent($"  • {otherLayer.displayName}"));
                    }
                }
            }

            menu.ShowAsContext();
        }

        private void HandleNameClicksAndDraw(Rect nameRect, SceneLayerDatabase.LayerDefinition layer)
        {
            var e = Event.current;
            if (nameRect.Contains(e.mousePosition) && e.type == EventType.Repaint)
            {
                m_hoverLayerGuidInPanel = layer.guid;
            }
            if (m_draggingObj)
            {
                DrawNameFieldOrLabel(nameRect, layer);
                return;
            }
            bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
            if (!m_renamingLayers.Contains(layer.guid) && nameRect.Contains(e.mousePosition) && GUIUtility.hotControl == 0 && !isPlayMode)
            {
                if (e.type == EventType.MouseDown && e.button == 0)
                {
                    if (e.clickCount == 2)
                    {
                        m_renamingLayers.Add(layer.guid);
                        m_renameBuffers[layer.guid] = layer.displayName;
                        SetSelectedLayer(layer.guid);
                        EditorApplication.RepaintHierarchyWindow();

                        var ctrl = "SL_Rename_" + layer.guid;
                        EditorApplication.delayCall += () =>
                        {
                            GUI.FocusControl(ctrl);
                            EditorGUI.FocusTextInControl(ctrl);
                        };
                        e.Use();
                        RequestRepaint();
                    }
                    else if (e.clickCount == 1)
                    {
                        SetSelectedLayer(layer.guid);
                        EditorApplication.RepaintHierarchyWindow();
                        e.Use();
                        RequestRepaint();
                    }
                }
            }

            DrawNameFieldOrLabel(nameRect, layer);
        }

        private void DrawNameFieldOrLabel(Rect nameRect, SceneLayerDatabase.LayerDefinition layer)
        {
            if (m_renamingLayers.Contains(layer.guid))
            {
                string ctrl = "SL_Rename_" + layer.guid;
                if (!m_renameBuffers.ContainsKey(layer.guid))
                    m_renameBuffers[layer.guid] = layer.displayName;

                GUI.SetNextControlName(ctrl);

                EditorGUI.BeginChangeCheck();
                string newBuf = EditorGUI.TextField(nameRect, m_renameBuffers[layer.guid]);
                if (EditorGUI.EndChangeCheck())
                {
                    m_renameBuffers[layer.guid] = newBuf;
                }

                var e = Event.current;

                if (e.type == EventType.KeyDown &&
                    (e.keyCode == KeyCode.Return || e.keyCode == KeyCode.KeypadEnter))
                {
                    CommitRename(layer);
                    e.Use();
                    return;
                }

                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
                {
                    CancelRename(layer);
                    e.Use();
                    return;
                }
                if (GUI.GetNameOfFocusedControl() == ctrl && !m_renameTextSelected.Contains(layer.guid))
                {
                    var textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    if (textEditor != null)
                    {
                        textEditor.SelectAll();
                        m_renameTextSelected.Add(layer.guid);
                    }
                }

                if (e.type == EventType.MouseDown && !nameRect.Contains(e.mousePosition))
                {
                    CommitRename(layer);
                    return;
                }

                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Tab)
                {
                    CommitRename(layer);
                    e.Use();
                    return;
                }
            }
            else
            {
                int count = GetCachedObjectCount(layer.guid);
                string displayText = count > 0 ? $"{layer.displayName} ({count})" : layer.displayName;

                var style = new GUIStyle(EditorStyles.label)
                {
                    alignment = TextAnchor.MiddleLeft,
                    fontStyle = (m_selectedLayerGuid == layer.guid ? FontStyle.Bold : FontStyle.Normal),
                };
                style.normal.textColor = EditorGUIUtility.isProSkin
                    ? Color.white
                    : Color.black;

                GUI.Label(nameRect, displayText, style);
            }
        }


        private void CommitRename(SceneLayerDatabase.LayerDefinition layer)
        {
            if (m_renameBuffers.TryGetValue(layer.guid, out var buf))
            {
                var trimmed = buf?.Trim();
                if (!string.IsNullOrEmpty(trimmed) && trimmed != layer.displayName)
                {
                    Undo.RecordObject(m_database, "Rename Layer");
                    layer.displayName = trimmed;
                    EditorUtility.SetDirty(m_database);
                }
            }
            m_renamingLayers.Remove(layer.guid);
            m_renameBuffers.Remove(layer.guid);
            m_renameTextSelected.Remove(layer.guid);
            GUI.FocusControl(null);
            RequestRepaint();
            EditorApplication.RepaintHierarchyWindow();
        }

        private void CancelRename(SceneLayerDatabase.LayerDefinition layer)
        {
            m_renamingLayers.Remove(layer.guid);
            m_renameBuffers.Remove(layer.guid);
            m_renameTextSelected.Remove(layer.guid);
            GUI.FocusControl(null);
            RequestRepaint();
        }
        private void DrawChildRow(SceneLayerDatabase.LayerDefinition layer, GameObject go,
                                  int iconW, float gap, float colorSquare, float dividerW, float rowH, float gapAfterCount,
                                  int rowIndex, int rowCount)
        {
            var r = EditorGUILayout.GetControlRect(false, rowH);
            var extendedRect = new Rect(r.x, r.y - 1f, r.width, r.height + 2f);
            if (!m_childRowRects.TryGetValue(layer.guid, out var rects))
                m_childRowRects[layer.guid] = rects = new List<Rect>();
            if (rowIndex >= rects.Count) rects.Add(r); else rects[rowIndex] = r;

            float x = r.x + m_foldW + gap;
            float y = r.y;
            float h = r.height;
            if (extendedRect.Contains(Event.current.mousePosition))
            {
                m_hoverObjectForHierarchy = go;
                m_hoverLayerGuidForHierarchy = layer.guid;
            }

            var e = Event.current;
            int dragId = GUIUtility.GetControlID("SL_ObjDrag".GetHashCode(), FocusType.Passive, extendedRect);

            int instanceId = go.GetInstanceID();
            if (!m_visibilityCache.TryGetValue(instanceId, out var visibility))
            {
                var svm = SceneVisibilityManager.instance;
                visibility = (svm.IsHidden(go), svm.IsPickingDisabled(go));
                m_visibilityCache[instanceId] = visibility;
            }
            bool hidden = visibility.hidden;
            bool pickingDisabled = visibility.pickingDisabled;

            var eyeRect = new Rect(x, y, iconW, h);
            IconButtonAbs(eyeRect, hidden ? "animationvisibilitytoggleoff" : "animationvisibilitytoggleon",
                hidden ? "Show" : "Hide", "👁",
                () =>
                {
                    var svm = SceneVisibilityManager.instance;
                    if (hidden) svm.Show(go, true); else svm.Hide(go, true);
                    m_visibilityCache.Remove(instanceId);
                    Repaint();
                });
            x += iconW + gap;

            var lockRect = new Rect(x, y, iconW, h);
            IconButtonAbs(lockRect, pickingDisabled ? "IN LockButton on" : "IN LockButton",
                pickingDisabled ? "Unlock" : "Lock", "🔒",
                () =>
                {
                    var svm = SceneVisibilityManager.instance;
                    if (pickingDisabled) svm.EnablePicking(go, true); else svm.DisablePicking(go, true);
                    m_visibilityCache.Remove(instanceId);
                    Repaint();
                });
            x += iconW + gap;

            x += gap;
            float rightButtonsWidth = (iconW + gap) * 2;
            float nameAvailable = r.width - (x - r.x) - rightButtonsWidth;
            if (nameAvailable < 60f) nameAvailable = 60f;
            var nameRect = new Rect(x, y, nameAvailable, h);

            float xSel = nameRect.xMax + gap;
            var selectRect = new Rect(xSel, y, iconW, h);
            float xRemove = xSel + iconW + gap;
            var removeRect = new Rect(xRemove, y, iconW, h);
            bool overButton = eyeRect.Contains(e.mousePosition) || lockRect.Contains(e.mousePosition) ||
                              selectRect.Contains(e.mousePosition) || removeRect.Contains(e.mousePosition);
            bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;
            if (!overButton && !isPlayMode)
            {
                switch (e.GetTypeForControl(dragId))
                {
                    case EventType.MouseDown:
                        if (e.button == 0 && extendedRect.Contains(e.mousePosition))
                        {
                            GUIUtility.hotControl = dragId;
                            m_dragObj = go;
                            m_dragObjSrcLayerGuid = layer.guid;
                            m_dragObjFromIndex = rowIndex;
                            m_dragObjMouseDownPos = e.mousePosition;
                            m_dragObjTargetLayerGuid = layer.guid;
                            m_dragObjInsertIndex = rowIndex;
                            m_dragObjPreviewInsertIndex = rowIndex;

                            e.Use();
                        }
                        break;

                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl == dragId)
                        {
                            if (!m_draggingObj && Vector2.Distance(e.mousePosition, m_dragObjMouseDownPos) > 2f)
                            {
                                m_draggingObj = true;
                            }
                            UpdateObjectDragPreview(e.mousePosition);
                            m_dragObjPreviewInsertIndex = m_dragObjInsertIndex;

                            e.Use();
                            RequestRepaint();
                        }
                        break;

                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == dragId)
                        {
                            GUIUtility.hotControl = 0;

                            if (m_draggingObj)
                            {
                                m_dragObjPendingDrop = true;
                                e.Use();
                            }
                            else
                            {
                                if (nameRect.Contains(m_dragObjMouseDownPos) || extendedRect.Contains(m_dragObjMouseDownPos))
                                {
                                    if (SceneLayerEditorSettings.ObjectRowClickBehavior == SceneLayerEditorSettings.ClickBehavior.Ping)
                                    {
                                        EditorGUIUtility.PingObject(go);
                                    }
                                    else
                                    {
                                        Selection.activeObject = go;
                                        EditorGUIUtility.PingObject(go);
                                    }
                                }
                                e.Use();
                            }
                        }
                        break;
                }
            }

            if (Event.current.type == EventType.ContextClick && extendedRect.Contains(Event.current.mousePosition))
            {
                ShowObjectContextMenu(go, layer);
                Event.current.Use();
                return;
            }

            DrawObjectNameBright(nameRect, go);
            IconButtonAbs(selectRect, "d_SceneViewOrtho", "Select", "✓",
                () => { Selection.activeObject = go; EditorGUIUtility.PingObject(go); });
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                string removeTooltip = isPlayMode ? "Cannot remove in Play Mode" : "Remove from Layer";
                IconButtonAbs(removeRect, "Toolbar Minus", removeTooltip, "−", () =>
                {
                    if (isPlayMode) return;
                    string goid = GOID(go);
                    if (layer.objectGlobalIds != null && layer.objectGlobalIds.Contains(goid))
                    {
                        Undo.RecordObject(m_database, "Remove From Layer");
                        layer.objectGlobalIds.Remove(goid);
                        EditorUtility.SetDirty(m_database);
                        RemoveFromOrder(layer.guid, goid);
                        UpdateCacheForObject(go, removeFromLayerGuid: layer.guid);

                        ShowNotification(new GUIContent($"Removed \"{go.name}\" from \"{layer.displayName}\""));
                    }
                    else
                    {
                        ShowNotification(new GUIContent("Object not in this layer"));
                    }
                });
            }
        }
        private static GUIStyle _iconOnlyCenterStyle;
        private static GUIStyle IconOnlyCenterStyle
        {
            get
            {
                if (_iconOnlyCenterStyle == null)
                {
                    _iconOnlyCenterStyle = new GUIStyle(GUIStyle.none)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        imagePosition = ImagePosition.ImageOnly
                    };
                }
                return _iconOnlyCenterStyle;
            }
        }

        private static void IconButtonAbs(Rect rect, string iconName, string tooltip, string fallbackText, System.Action onClick)
        {
            var gcIcon = EditorGUIUtility.IconContent(iconName);
            var content = (gcIcon != null && gcIcon.image != null)
                ? new GUIContent(gcIcon.image, tooltip)
                : new GUIContent(fallbackText, tooltip);

            var oldIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));

            var oldColor = GUI.color;
            GUI.color = EditorGUIUtility.isProSkin
                ? Color.white
                : new Color(0.2f, 0.2f, 0.2f, 1f);
            var cleanButtonStyle = new GUIStyle(GUIStyle.none)
            {
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageOnly,
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0)
            };

            if (GUI.Button(rect, content, cleanButtonStyle))
                onClick?.Invoke();

            GUI.color = oldColor;
            EditorGUIUtility.SetIconSize(oldIconSize);
        }

        private static Color ComputeReadableTint(Color c)
        {
            Color adjusted = c;
            float L = 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;
            if (L < 0.10f)
            {
                adjusted = Color.Lerp(c, Color.white, 0.15f);
            }
            adjusted.a = EditorGUIUtility.isProSkin ? 0.55f : 0.65f;
            return adjusted;
        }
        private static Color GetSeparatorColorDark()
        {
            return EditorGUIUtility.isProSkin
                ? new Color(0f, 0f, 0f, 0.40f)
                : new Color(0f, 0f, 0f, 0.25f);
        }

        private string GenerateNextLayerName()
        {
            int highestNumber = 0;

            foreach (var layer in m_database.layers)
            {
                if (layer.displayName.StartsWith("Layer "))
                {
                    string numberPart = layer.displayName.Substring(6);
                    if (int.TryParse(numberPart, out int number))
                    {
                        highestNumber = Mathf.Max(highestNumber, number);
                    }
                }
            }
            return $"Layer {highestNumber + 1}";
        }

        private SceneLayerDatabase.LayerDefinition GetLayerByGuid(string guid)
        {
            if (string.IsNullOrEmpty(guid) || m_database?.layers == null) return null;
            for (int i = 0; i < m_database.layers.Count; i++)
            {
                if (m_database.layers[i].guid == guid) return m_database.layers[i];
            }
            return null;
        }
        private bool HandleAssignDrag(Rect headerRect, SceneLayerDatabase.LayerDefinition layer)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return false;
            }

            var e = Event.current;

            if (e.type == EventType.DragExited)
            {
                m_dragAssignHoverLayerGuid = null;
                m_extDragActive = false;
                m_extDragTargetLayerGuid = null;
                m_extDragInsertIndex = -1;
                RequestRepaint();
                return false;
            }

            Vector2 mouse = Event.current.mousePosition;
            if (!headerRect.Contains(mouse))
            {
                if (m_dragAssignHoverLayerGuid == layer.guid)
                {
                    m_dragAssignHoverLayerGuid = null;
                    RequestRepaint();
                }
                return false;
            }
            if (m_draggingObj)
            {
                m_dragAssignHoverLayerGuid = layer.guid;
                m_extDragActive = true;
                m_extDragTargetLayerGuid = layer.guid;
                m_extDragInsertIndex = 0;

                RequestRepaint();
                return true;
            }
            var refs = DragAndDrop.objectReferences;
            var sceneObjects = new List<GameObject>();
            var prefabAssets = new List<GameObject>();

            if (refs != null)
            {
                foreach (var r in refs)
                {
                    if (r is GameObject go)
                    {
                        if (go.scene.IsValid()) sceneObjects.Add(go);
                        else if (AssetDatabase.Contains(go)) prefabAssets.Add(go);
                    }
                    else if (r is Component c && c)
                    {
                        if (c.gameObject.scene.IsValid()) sceneObjects.Add(c.gameObject);
                    }
                }
            }

            bool hasPayload = sceneObjects.Count > 0 || prefabAssets.Count > 0;

            if (e.type == EventType.DragUpdated)
            {
                m_extDragActive = true;
                m_extDragTargetLayerGuid = layer.guid;
                m_extDragInsertIndex = 0;

                if (hasPayload)
                {
                    DragAndDrop.visualMode = (sceneObjects.Count > 0)
                        ? DragAndDropVisualMode.Link
                        : DragAndDropVisualMode.Copy;
                }

                e.Use();
                RequestRepaint();
                return true;
            }

            if (!hasPayload) return false;

            m_dragAssignHoverLayerGuid = layer.guid;

            if (e.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                if (sceneObjects.Count > 0)
                {
                    var (added, already) = AssignObjectsToLayerWithCounts(sceneObjects, layer, m_database);
                    if (added > 0)
                    {
                        ShowNotification(new GUIContent("Assigned " + added + " to " + layer.displayName));
                        m_layerFoldouts[layer.guid] = true;
                    }
                    if (already > 0 && added == 0)
                    {
                        ShowNotification(new GUIContent("Object already in layer"));
                    }
                    EditorApplication.RepaintHierarchyWindow();
                    RequestRepaint();
                    e.Use();
                    return true;
                }

                if (prefabAssets.Count > 0)
                {
                    var popupRect = new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0);
                    PopupWindow.Show(popupRect, new PrefabDropPopup(prefabAssets, layer, m_database));
                    e.Use();
                    return true;
                }
            }

            return true;
        }
        private void UpdateDragInsertIndexPreciseScreen(Vector2 mouseGuiPos)
        {
            int count = m_database.layers.Count;
            if (count == 0) { m_dragInsertIndex = 0; return; }
            var rects = new List<(int idx, Rect r)>(count);
            for (int i = 0; i < count; i++)
            {
                var guiR = m_headerRects[i];
                if (guiR.height <= 0.01f) continue;
                rects.Add((i, guiR));
            }
            if (rects.Count == 0) { m_dragInsertIndex = 0; return; }

            rects.Sort((a, b) => a.r.yMin.CompareTo(b.r.yMin));
            float mouseY = mouseGuiPos.y;

            var first = rects[0].r;
            float firstMid = (first.yMin + first.yMax) * 0.5f;
            if (mouseY <= firstMid) { m_dragInsertIndex = 0; return; }

            for (int i = 0; i < rects.Count - 1; i++)
            {
                var a = rects[i].r;
                var b = rects[i + 1].r;

                if (mouseY <= a.yMax)
                {
                    float midA = (a.yMin + a.yMax) * 0.5f;
                    m_dragInsertIndex = (mouseY < midA) ? rects[i].idx : rects[i].idx + 1;
                    return;
                }

                if (mouseY < b.yMin)
                {
                    m_dragInsertIndex = rects[i].idx + 1;
                    return;
                }
            }

            var last = rects[rects.Count - 1].r;
            float lastMid = (last.yMin + last.yMax) * 0.5f;
            if (mouseY <= last.yMax)
                m_dragInsertIndex = (mouseY < lastMid) ? rects[rects.Count - 1].idx : rects[rects.Count - 1].idx + 1;
            else
                m_dragInsertIndex = m_database.layers.Count;
        }

        private void CommitLayerReorder()
        {
            int count = m_database.layers.Count;
            if (m_dragFromIndex < 0 || m_dragFromIndex >= count) return;

            int to = Mathf.Clamp(m_dragInsertIndex, 0, count);
            if (to > m_dragFromIndex) to--;

            if (to != m_dragFromIndex)
            {
                Undo.RecordObject(m_database, "Reorder Layers");
                var item = m_database.layers[m_dragFromIndex];
                m_database.layers.RemoveAt(m_dragFromIndex);
                m_database.layers.Insert(to, item);
                EditorUtility.SetDirty(m_database);
            }

            RequestRepaint();
        }

        private Color ComputeHierarchyTintFromLayer(Color layerColor)
        {
            var t = ComputeReadableTint(layerColor);
            t.a = EditorGUIUtility.isProSkin ? 0.12f : 0.14f;
            return t;
        }
        private static GUIStyle _goLabelStyle;
        private static GUIStyle GoLabelStyle
        {
            get
            {
                if (_goLabelStyle == null)
                {
                    _goLabelStyle = new GUIStyle(EditorStyles.label)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        imagePosition = ImagePosition.ImageLeft,
                        padding = new RectOffset(2, 0, 0, 0)
                    };
                }
                _goLabelStyle.normal.textColor = EditorGUIUtility.isProSkin
                    ? new Color(0.95f, 0.95f, 0.95f, 0.95f)
                    : new Color(0.10f, 0.10f, 0.10f, 0.95f);
                return _goLabelStyle;
            }
        }

        private static void DrawObjectNameBright(Rect rect, GameObject go)
        {
            var content = EditorGUIUtility.ObjectContent(go, typeof(GameObject));
            if (content == null) content = new GUIContent(go ? go.name : "(null)");
            else content.text = go.name;

            var oldIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(new Vector2(16, 16));
            var oldColor = GUI.color;
            if (go != null && !go.activeInHierarchy)
            {
                GUI.color = EditorGUIUtility.isProSkin
                    ? new Color(1f, 1f, 1f, 0.5f)
                    : new Color(0f, 0f, 0f, 0.5f);
            }
            else
            {
                GUI.color = EditorGUIUtility.isProSkin
                    ? Color.white
                    : Color.black;
            }

            GUI.Label(rect, content, GoLabelStyle);

            GUI.color = oldColor;
            EditorGUIUtility.SetIconSize(oldIconSize);
        }
        internal static void ApplyLayerToPrefabAssets(IEnumerable<GameObject> prefabAssets,
                                                      SceneLayerDatabase.LayerDefinition layer,
                                                      SceneLayerDatabase database,
                                                      bool updateExistingInstances)
        {
            if (prefabAssets == null || layer == null || database == null) return;

            if (updateExistingInstances)
            {
                var all = UnityEngine.Object.FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                var toAssign = new List<GameObject>();
                foreach (var prefab in prefabAssets.Distinct())
                {
                    if (!prefab) continue;

                    foreach (var go in all)
                    {
                        if (!go || !go.scene.IsValid()) continue;
                        var origin = PrefabUtility.GetCorrespondingObjectFromOriginalSource(go);
                        if (origin == prefab) toAssign.Add(go);
                    }
                }
                AssignObjectsToLayer(toAssign, layer, database);
            }

            EditorApplication.RepaintHierarchyWindow();
            if (m_instance != null) m_instance.Repaint();
        }

        internal static void InstantiatePrefabsInScene(IEnumerable<GameObject> prefabAssets,
                                                       SceneLayerDatabase.LayerDefinition layer,
                                                       SceneLayerDatabase database)
        {
            if (prefabAssets == null || layer == null || database == null) return;

            var sv = SceneView.lastActiveSceneView;
            Vector3 basePos = sv ? sv.pivot : Vector3.zero;

            Undo.IncrementCurrentGroup();
            int group = Undo.GetCurrentGroup();

            GameObject last = null;
            int i = 0;
            foreach (var prefab in prefabAssets.Distinct())
            {
                if (!prefab) continue;
                var obj = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                if (!obj) continue;

                obj.transform.position = basePos + new Vector3((i % 5) * 2f, 0f, (i / 5) * 2f);
                Undo.RegisterCreatedObjectUndo(obj, "Instantiate Prefab");

                AssignObjectsToLayer(new[] { obj }, layer, database);
                last = obj;
                i++;
            }

            Undo.CollapseUndoOperations(group);

            if (last)
            {
                Selection.activeObject = last;
                EditorGUIUtility.PingObject(last);
            }

            EditorApplication.RepaintHierarchyWindow();
            if (m_instance != null) m_instance.Repaint();
        }

        private void HandleSeparatorDragAssign(Rect separatorRect, SceneLayerDatabase.LayerDefinition layer)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            var e = Event.current;
            Vector2 mouse = Event.current.mousePosition;

            if (!separatorRect.Contains(mouse))
            {
                return;
            }
            if (m_draggingObj)
            {
                m_dragObjTargetLayerGuid = layer.guid;
                m_dragObjInsertIndex = 0;
                m_dragAssignHoverLayerGuid = layer.guid;
                if (e.type == EventType.MouseUp && e.button == 0)
                {
                    m_dragObjPendingDrop = true;
                    e.Use();
                }

                RequestRepaint();
                return;
            }
            var refs = DragAndDrop.objectReferences;
            if (refs == null || refs.Length == 0) return;

            var sceneObjects = new List<GameObject>();
            var prefabAssets = new List<GameObject>();

            foreach (var r in refs)
            {
                if (r is GameObject go)
                {
                    if (go.scene.IsValid()) sceneObjects.Add(go);
                    else if (AssetDatabase.Contains(go)) prefabAssets.Add(go);
                }
                else if (r is Component c && c)
                {
                    if (c.gameObject.scene.IsValid()) sceneObjects.Add(c.gameObject);
                }
            }

            bool hasPayload = sceneObjects.Count > 0 || prefabAssets.Count > 0;

            if (e.type == EventType.DragUpdated && hasPayload)
            {
                DragAndDrop.visualMode = (sceneObjects.Count > 0)
                    ? DragAndDropVisualMode.Link
                    : DragAndDropVisualMode.Copy;
                m_dragAssignHoverLayerGuid = layer.guid;
                e.Use();
                RequestRepaint();
                return;
            }

            if (hasPayload)
            {
                m_dragAssignHoverLayerGuid = layer.guid;

                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    if (sceneObjects.Count > 0)
                    {
                        var (added, already) = AssignObjectsToLayerWithCounts(sceneObjects, layer, m_database);

                        foreach (var go in sceneObjects.Distinct())
                        {
                            InsertIntoOrder(layer.guid, GOID(go), 0);
                        }

                        if (added > 0)
                        {
                            ShowNotification(new GUIContent("Assigned " + added + " to " + layer.displayName));
                        }
                        if (already > 0 && added == 0)
                        {
                            ShowNotification(new GUIContent("Object already in layer"));
                        }

                        EditorApplication.RepaintHierarchyWindow();
                        RequestRepaint();
                        e.Use();
                        return;
                    }

                    if (prefabAssets.Count > 0)
                    {
                        var popupRect = new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0);
                        PopupWindow.Show(popupRect, new PrefabDropPopup(prefabAssets, layer, m_database));
                        e.Use();
                        return;
                    }
                }
            }
        }
        private void HandleEmptyLayerDragAssign(Rect emptyRect, SceneLayerDatabase.LayerDefinition layer)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (m_dragAssignHoverLayerGuid == layer.guid)
                {
                    m_dragAssignHoverLayerGuid = null;
                    RequestRepaint();
                }
                return;
            }
            var e = Event.current;
            Vector2 mouse = Event.current.mousePosition;

            if (!emptyRect.Contains(mouse))
            {
                if (m_dragAssignHoverLayerGuid == layer.guid)
                {
                    m_dragAssignHoverLayerGuid = null;
                    RequestRepaint();
                }
                return;
            }
            if (m_draggingObj)
            {
                m_dragObjTargetLayerGuid = layer.guid;
                m_dragObjInsertIndex = 0;
                m_dragAssignHoverLayerGuid = layer.guid;
                if (e.type == EventType.MouseUp && e.button == 0)
                {
                    m_dragObjPendingDrop = true;
                    e.Use();
                }

                RequestRepaint();
                return;
            }
            var refs = DragAndDrop.objectReferences;
            var sceneObjects = new List<GameObject>();
            var prefabAssets = new List<GameObject>();

            if (refs != null)
            {
                foreach (var r in refs)
                {
                    if (r is GameObject go)
                    {
                        if (go.scene.IsValid()) sceneObjects.Add(go);
                        else if (AssetDatabase.Contains(go)) prefabAssets.Add(go);
                    }
                    else if (r is Component c && c)
                    {
                        if (c.gameObject.scene.IsValid()) sceneObjects.Add(c.gameObject);
                    }
                }
            }

            bool hasPayload = sceneObjects.Count > 0 || prefabAssets.Count > 0;

            if (e.type == EventType.DragUpdated && hasPayload)
            {
                DragAndDrop.visualMode = (sceneObjects.Count > 0)
                    ? DragAndDropVisualMode.Link
                    : DragAndDropVisualMode.Copy;
                m_dragAssignHoverLayerGuid = layer.guid;
                e.Use();
                RequestRepaint();
                return;
            }

            if (hasPayload)
            {
                m_dragAssignHoverLayerGuid = layer.guid;

                if (e.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    if (sceneObjects.Count > 0)
                    {
                        var (added, already) = AssignObjectsToLayerWithCounts(sceneObjects, layer, m_database);
                        if (added > 0)
                        {
                            ShowNotification(new GUIContent("Assigned " + added + " to " + layer.displayName));
                            SetLayerFoldout(layer.guid, true);
                        }
                        if (already > 0 && added == 0)
                        {
                            ShowNotification(new GUIContent("Object already in layer"));
                        }
                        EditorApplication.RepaintHierarchyWindow();
                        RequestRepaint();
                        e.Use();
                        return;
                    }

                    if (prefabAssets.Count > 0)
                    {
                        var popupRect = new Rect(e.mousePosition.x, e.mousePosition.y, 0, 0);
                        PopupWindow.Show(popupRect, new PrefabDropPopup(prefabAssets, layer, m_database));
                        e.Use();
                        return;
                    }
                }
            }
            if (m_dragAssignHoverLayerGuid == layer.guid && Event.current.type == EventType.Repaint)
            {
                var hl = EditorGUIUtility.isProSkin
                    ? new Color(0.35f, 0.60f, 1f, 0.18f)
                    : new Color(0.20f, 0.50f, 1f, 0.20f);

                EditorGUI.DrawRect(emptyRect, hl);
                var b = new Color(hl.r, hl.g, hl.b, hl.a * 1.7f);
                EditorGUI.DrawRect(new Rect(emptyRect.x, emptyRect.y, emptyRect.width, 2f), b);
                EditorGUI.DrawRect(new Rect(emptyRect.x, emptyRect.yMax - 2f, emptyRect.width, 2f), b);
                EditorGUI.DrawRect(new Rect(emptyRect.x, emptyRect.y, 2f, emptyRect.height), b);
                EditorGUI.DrawRect(new Rect(emptyRect.xMax - 2f, emptyRect.y, 2f, emptyRect.height), b);
            }
        }

        private static void AssignObjectsToLayer(IEnumerable<GameObject> objs, SceneLayerDatabase.LayerDefinition layer, SceneLayerDatabase database)
        {
            AssignObjectsToLayerWithCounts(objs, layer, database);
        }

        private static (int added, int already) AssignObjectsToLayerWithCounts(IEnumerable<GameObject> objs, SceneLayerDatabase.LayerDefinition layer, SceneLayerDatabase database)
        {
            if (objs == null || layer == null || database == null) return (0, 0);

            Undo.RecordObject(database, "Assign To Layer");

            int added = 0, already = 0;

            if (layer.objectGlobalIds == null)
                layer.objectGlobalIds = new List<string>();

            foreach (var go in objs.Distinct())
            {
                if (!go || !go.scene.IsValid()) continue;

                string goid = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();

                if (!layer.objectGlobalIds.Contains(goid))
                {
                    layer.objectGlobalIds.Add(goid);
                    UpdateCacheForObject(go, addToLayerGuid: layer.guid);

                    added++;
                }
                else
                {
                    already++;
                }
            }

            EditorUtility.SetDirty(database);
            if (added > 0 && m_instance != null)
            {
                m_instance.m_orderedObjectsCache.Remove(layer.guid);
                m_instance.m_layerObjectCounts.Remove(layer.guid);
                m_instance.Repaint();
            }

            return (added, already);
        }
        internal static int ScanSceneForLayerRules(SceneLayerDatabase db, SceneLayerDatabase.LayerDefinition layer)
        {
            if (db == null || layer == null) return 0;
            var matches = GatherObjectsMatchingRules(layer);
            if (matches.Count > 0)
            {
                AssignObjectsToLayer(matches, layer, db);
                if (m_instance != null)
                {
                    m_instance.m_layerFoldouts[layer.guid] = true;
                    m_instance.Repaint();
                }
                EditorApplication.RepaintHierarchyWindow();
            }
            return matches.Count;
        }

        internal static List<GameObject> GatherObjectsMatchingRules(SceneLayerDatabase.LayerDefinition layer)
        {
            var results = new List<GameObject>();
            if (layer == null || layer.autoRules == null || layer.autoRules.Count == 0) return results;

            foreach (var rule in layer.autoRules)
            {
                var t = ResolveComponentType(rule?.componentTypeName);
                if (t == null) continue;

                var found = UnityEngine.Object.FindObjectsByType(t, FindObjectsInactive.Include, FindObjectsSortMode.None);
                foreach (var o in found)
                {
                    if (o is Component c && c && c.gameObject) results.Add(c.gameObject);
                }
            }

            return results.Distinct().ToList();
        }
        internal static List<GameObject> GatherObjectsMatchingRulesFromList(SceneLayerDatabase.LayerDefinition layer, List<GameObject> objectsToCheck)
        {
            var results = new List<GameObject>();
            if (layer == null || layer.autoRules == null || layer.autoRules.Count == 0) return results;

            foreach (var rule in layer.autoRules)
            {
                var t = ResolveComponentType(rule?.componentTypeName);
                if (t == null) continue;

                foreach (var go in objectsToCheck)
                {
                    if (go != null && go.GetComponent(t) != null)
                    {
                        results.Add(go);
                    }
                }
            }

            return results.Distinct().ToList();
        }

        internal static Type ResolveComponentType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName)) return null;
            var t = Type.GetType(typeName);
            if (t != null && typeof(Component).IsAssignableFrom(t)) return t;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var tt = asm.GetType(typeName);
                    if (tt != null && typeof(Component).IsAssignableFrom(tt)) return tt;
                }
                catch { }
            }
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var tt = asm.GetTypes().FirstOrDefault(x => x.Name == typeName && typeof(Component).IsAssignableFrom(x));
                    if (tt != null) return tt;
                }
                catch { }
            }
            return null;
        }
        private static string GOID(GameObject go)
            => GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();

        private void EnsureOrderListExists(string layerGuid)
        {
            if (!m_orderByLayer.ContainsKey(layerGuid))
                m_orderByLayer[layerGuid] = new List<string>();
        }

        private void LoadAllOrders()
        {
            m_orderByLayer.Clear();
            if (m_database?.layers == null) return;
            foreach (var l in m_database.layers)
            {
                var raw = EditorPrefs.GetString(ORDER_PREF_PREFIX + l.guid, "");
                if (!string.IsNullOrEmpty(raw))
                    m_orderByLayer[l.guid] = raw.Split('|').Where(s => !string.IsNullOrEmpty(s)).ToList();
            }
        }

        private void SaveAllOrders()
        {
            if (m_database?.layers == null) return;
            foreach (var l in m_database.layers)
            {
                if (m_orderByLayer.TryGetValue(l.guid, out var list) && list != null)
                    EditorPrefs.SetString(ORDER_PREF_PREFIX + l.guid, string.Join("|", list));
                else
                    EditorPrefs.DeleteKey(ORDER_PREF_PREFIX + l.guid);
            }
        }

        private void RemoveFromOrder(string layerGuid, string gid)
        {
            if (m_orderByLayer.TryGetValue(layerGuid, out var list))
            {
                list.Remove(gid);
                SaveAllOrders();
            }
        }

        private void InsertIntoOrder(string layerGuid, string gid, int index)
        {
            EnsureOrderListExists(layerGuid);
            var list = m_orderByLayer[layerGuid];
            if (list.Contains(gid)) list.Remove(gid);
            list.Insert(Mathf.Clamp(index, 0, list.Count), gid);
            SaveAllOrders();
        }

        private void MoveInOrder(string layerGuid, string gid, int newIndex)
        {
            EnsureOrderListExists(layerGuid);
            var list = m_orderByLayer[layerGuid];
            if (!list.Contains(gid)) list.Add(gid);
            list.Remove(gid);
            list.Insert(Mathf.Clamp(newIndex, 0, list.Count), gid);
            SaveAllOrders();
        }

        private List<GameObject> GetOrderedObjects(SceneLayerDatabase.LayerDefinition layer)
        {
            int currentFrame = Time.frameCount;
            if (currentFrame != m_lastCacheFrame)
            {
                m_frameCache.Clear();
                m_lastCacheFrame = currentFrame;
            }

            if (m_frameCache.TryGetValue(layer.guid, out var frameCache))
            {
                return frameCache;
            }

            if (m_orderedObjectsCache.TryGetValue(layer.guid, out var cached))
            {
                cached.RemoveAll(go => go == null);
                m_frameCache[layer.guid] = cached;
                return cached;
            }

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (m_layerObjectCache.TryGetValue(layer.guid, out var objects))
                {
                    var list = objects.Where(go => go != null).ToList();

                    if (m_orderByLayer.TryGetValue(layer.guid, out var ord) && ord != null && ord.Count > 0)
                    {
                        var idx = new Dictionary<string, int>(ord.Count);
                        for (int i = 0; i < ord.Count; i++) idx[ord[i]] = i;

                        list.Sort((a, b) =>
                        {
                            if (a == null && b == null) return 0;
                            if (a == null) return 1;
                            if (b == null) return -1;

                            var ga = GOID(a); var gb = GOID(b);
                            bool ha = idx.TryGetValue(ga, out int ia);
                            bool hb = idx.TryGetValue(gb, out int ib);
                            if (ha && hb) return ia.CompareTo(ib);
                            if (ha) return -1;
                            if (hb) return 1;
                            return EditorUtility.NaturalCompare(a.name, b.name);
                        });
                    }

                    m_orderedObjectsCache[layer.guid] = list;
                    m_frameCache[layer.guid] = list;
                    return list;
                }

                var emptyList = new List<GameObject>();
                m_frameCache[layer.guid] = emptyList;
                return emptyList;
            }

            RebuildCacheIfNeeded();

            var resultList = m_layerObjectCache.TryGetValue(layer.guid, out var cachedObjects)
                ? cachedObjects.Where(go => go != null).ToList()
                : new List<GameObject>();

            if (!m_orderByLayer.TryGetValue(layer.guid, out var ord2) || ord2 == null || ord2.Count == 0)
            {
                m_orderedObjectsCache[layer.guid] = resultList;
                m_frameCache[layer.guid] = resultList;
                return resultList;
            }

            var idx2 = new Dictionary<string, int>(ord2.Count);
            for (int i = 0; i < ord2.Count; i++) idx2[ord2[i]] = i;

            resultList.Sort((a, b) =>
            {
                if (a == null && b == null) return 0;
                if (a == null) return 1;
                if (b == null) return -1;

                var ga = GOID(a); var gb = GOID(b);
                bool ha = idx2.TryGetValue(ga, out int ia);
                bool hb = idx2.TryGetValue(gb, out int ib);
                if (ha && hb) return ia.CompareTo(ib);
                if (ha) return -1;
                if (hb) return 1;
                return EditorUtility.NaturalCompare(a.name, b.name);
            });

            m_orderedObjectsCache[layer.guid] = resultList;
            m_frameCache[layer.guid] = resultList;
            return resultList;
        }
        private void UpdateObjectDragPreview(Vector2 mouseGuiPos)
        {
            m_dragObjTargetLayerGuid = null;
            m_dragObjInsertIndex = -1;

            Vector2 mouse = mouseGuiPos;
            for (int i = 0; i < m_database.layers.Count; i++)
            {
                if (m_headerRects.Length <= i) break;
                var header = m_headerRects[i];
                var expandedHeader = new Rect(header.x, header.y - 2f, header.width, header.height + 4f);

                if (expandedHeader.Contains(mouse))
                {
                    m_dragObjTargetLayerGuid = m_database.layers[i].guid;
                    m_dragObjInsertIndex = 0;
                    return;
                }
            }

            for (int i = 0; i < m_database.layers.Count; i++)
            {
                var layer = m_database.layers[i];
                if (!m_layerFoldouts.TryGetValue(layer.guid, out var open) || !open) continue;
                if (!m_childRowRects.TryGetValue(layer.guid, out var rows) || rows.Count == 0) continue;

                var first = rows[0];
                var last = rows[rows.Count - 1];
                var area = Rect.MinMaxRect(first.xMin, first.yMin, last.xMax, last.yMax);

                if (!area.Contains(mouse)) continue;

                m_dragObjTargetLayerGuid = layer.guid;
                for (int j = 0; j < rows.Count; j++)
                {
                    var r = rows[j];
                    float zoneTop, zoneBottom;

                    if (j == 0)
                    {
                        zoneTop = r.yMin;
                        zoneBottom = r.yMin + (r.height * 0.75f);
                    }
                    else
                    {
                        var prevRow = rows[j - 1];
                        zoneTop = prevRow.yMin + (prevRow.height * 0.75f);
                        zoneBottom = r.yMin + (r.height * 0.75f);
                    }

                    if (mouse.y >= zoneTop && mouse.y < zoneBottom)
                    {
                        m_dragObjInsertIndex = j;
                        return;
                    }
                }
                m_dragObjInsertIndex = rows.Count;
                return;
            }
        }


        private void CommitObjectDrag()
        {
            if (m_dragObj == null || string.IsNullOrEmpty(m_dragObjTargetLayerGuid)) return;

            string gid = GOID(m_dragObj);

            if (m_dragObjTargetLayerGuid == m_dragObjSrcLayerGuid)
            {
                int adjustedIndex = m_dragObjInsertIndex;
                if (adjustedIndex > m_dragObjFromIndex)
                {
                    adjustedIndex--;
                }
                MoveInOrder(m_dragObjSrcLayerGuid, gid, adjustedIndex);
                m_orderedObjectsCache.Remove(m_dragObjSrcLayerGuid);

                m_frameCache.Clear();
            }
            else
            {
                Undo.RecordObject(m_database, "Move To Layer");
                var srcLayer = GetLayerByGuid(m_dragObjSrcLayerGuid);
                if (srcLayer?.objectGlobalIds != null)
                {
                    srcLayer.objectGlobalIds.Remove(gid);
                }
                var targetLayer = GetLayerByGuid(m_dragObjTargetLayerGuid);
                if (targetLayer != null)
                {
                    if (targetLayer.objectGlobalIds == null)
                        targetLayer.objectGlobalIds = new List<string>();

                    if (targetLayer.objectGlobalIds.Contains(gid))
                    {
                        ShowNotification(new GUIContent("Object already in layer"));
                    }
                    else
                    {
                        targetLayer.objectGlobalIds.Add(gid);
                        UpdateCacheForObject(m_dragObj, removeFromLayerGuid: m_dragObjSrcLayerGuid, addToLayerGuid: m_dragObjTargetLayerGuid);

                        EditorUtility.SetDirty(m_database);

                        RemoveFromOrder(m_dragObjSrcLayerGuid, gid);
                        InsertIntoOrder(m_dragObjTargetLayerGuid, gid, m_dragObjInsertIndex);

                        var svm = SceneVisibilityManager.instance;
                        if (targetLayer.defaultPickable)
                        {
                            svm.EnablePicking(m_dragObj, false);
                        }
                        else
                        {
                            svm.DisablePicking(m_dragObj, false);
                        }

                        ShowNotification(new GUIContent($"Moved \"{m_dragObj.name}\" to \"{targetLayer.displayName}\""));

                        m_layerFoldouts[m_dragObjTargetLayerGuid] = true;
                    }
                }

                m_frameCache.Clear();
            }

            EditorApplication.RepaintHierarchyWindow();
            Repaint();
        }
        private void HandleExternalAssignOverChildren(SceneLayerDatabase.LayerDefinition layer)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            if (!m_childRowRects.TryGetValue(layer.guid, out var rows) || rows.Count == 0) return;

            var e = Event.current;
            var refs = DragAndDrop.objectReferences;
            if (refs == null || refs.Length == 0) return;
            var area = Rect.MinMaxRect(rows[0].xMin, rows[0].yMin,
                                      rows[rows.Count - 1].xMax,
                                      rows[rows.Count - 1].yMax);

            if (!area.Contains(e.mousePosition)) return;
            int insert = 0;
            float y = e.mousePosition.y;
            for (int j = 0; j < rows.Count; j++)
            {
                float mid = (rows[j].yMin + rows[j].yMax) * 0.5f;
                if (y < mid) { insert = j; break; }
                insert = j + 1;
            }
            var gos = new List<GameObject>();
            foreach (var r in refs)
            {
                if (r is GameObject g && g.scene.IsValid()) gos.Add(g);
                else if (r is Component c && c && c.gameObject.scene.IsValid()) gos.Add(c.gameObject);
            }
            if (gos.Count == 0) return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Link;

            if (e.type == EventType.DragUpdated)
            {
                m_extDragActive = true;
                m_extDragTargetLayerGuid = layer.guid;
                m_extDragInsertIndex = insert;
                e.Use();
                RequestRepaint();
                return;
            }

            if (e.type == EventType.DragPerform)
            {
                DragAndDrop.AcceptDrag();

                var (added, already) = AssignObjectsToLayerWithCounts(gos, layer, m_database);
                foreach (var go in gos.Distinct())
                    InsertIntoOrder(layer.guid, GOID(go), insert++);

                if (added > 0)
                {
                    m_layerFoldouts[layer.guid] = true;
                    ShowNotification(new GUIContent("Assigned " + added + " to " + layer.displayName));
                }
                if (already > 0 && added == 0)
                    ShowNotification(new GUIContent("Object already in layer"));

                EditorApplication.RepaintHierarchyWindow();
                RequestRepaint();
                e.Use();
            }
        }
        private static void OnHierarchyItemGUI(int instanceID, Rect selectionRect)
        {
            var win = m_instance;
            if (win == null) return;
            if (!SceneLayerEditorSettings.UseSceneColors) return;

            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (!go) return;
            var layerGuids = SceneLayerController.GetLayersForObject(go, win.m_database);
            if (layerGuids == null || layerGuids.Count == 0) return;

            bool isHoveredInChildRow = (win.m_hoverObjectForHierarchy == go);
            bool isInHoveredLayer = false;
            bool isInSelectedLayer = false;
            bool shouldShowLayerColors = SceneLayerEditorSettings.UseSceneColors;

            if (!string.IsNullOrEmpty(win.m_hoverLayerGuidInPanel))
            {
                isInHoveredLayer = layerGuids.Contains(win.m_hoverLayerGuidInPanel);
            }
            if (!string.IsNullOrEmpty(win.m_selectedLayerGuid))
            {
                isInSelectedLayer = layerGuids.Contains(win.m_selectedLayerGuid);
            }
            bool shouldHighlight = isHoveredInChildRow || isInHoveredLayer || (shouldShowLayerColors && isInSelectedLayer);

            if (!shouldHighlight) return;

            Color col;

            if (shouldShowLayerColors && layerGuids.Count > 0)
            {
                if (layerGuids.Count == 1)
                {
                    var layer = win.GetLayerByGuid(layerGuids[0]);
                    col = layer != null ? win.ComputeHierarchyTintFromLayer(layer.color) : GetNeutralHierarchyColor(isHoveredInChildRow || isInHoveredLayer);
                }
                else
                {
                    var colors = new List<Color>();
                    foreach (var guid in layerGuids.Take(4))
                    {
                        var layer = win.GetLayerByGuid(guid);
                        if (layer != null) colors.Add(layer.color);
                    }

                    if (colors.Count > 1)
                    {
                        var splitMode = SceneLayerEditorSettings.MultiLayerSplitMode;
                        DrawMultiLayerHighlight(selectionRect, colors, isHoveredInChildRow || isInHoveredLayer, splitMode);
                        return;
                    }
                    else if (colors.Count == 1)
                    {
                        col = win.ComputeHierarchyTintFromLayer(colors[0]);
                    }
                    else
                    {
                        col = GetNeutralHierarchyColor(isHoveredInChildRow || isInHoveredLayer);
                    }
                }
            }
            else
            {
                col = GetNeutralHierarchyColor(isHoveredInChildRow || isInHoveredLayer);
            }

            if (isHoveredInChildRow || isInHoveredLayer)
            {
                col.a = Mathf.Clamp01(col.a * 1.4f);
            }

            EditorGUI.DrawRect(selectionRect, col);
        }

        private static void DrawMultiLayerHighlight(Rect rect, List<Color> layerColors, bool isHovered, SceneLayerEditorSettings.MultiLayerSplit splitMode)
        {
            if (layerColors.Count <= 1)
            {
                if (layerColors.Count == 1)
                {
                    var col = layerColors[0];
                    col.a = 0.15f;
                    if (isHovered) col.a *= 1.3f;
                    EditorGUI.DrawRect(rect, col);
                }
                return;
            }

            if (splitMode == SceneLayerEditorSettings.MultiLayerSplit.Vertical)
            {
                float stripeWidth = rect.width / layerColors.Count;
                for (int i = 0; i < layerColors.Count; i++)
                {
                    var stripeRect = new Rect(rect.x + (i * stripeWidth), rect.y, stripeWidth, rect.height);
                    var col = layerColors[i];
                    col.a = 0.15f;
                    if (isHovered) col.a *= 1.3f;
                    EditorGUI.DrawRect(stripeRect, col);
                }
            }
            else
            {
                float stripeHeight = rect.height / layerColors.Count;
                for (int i = 0; i < layerColors.Count; i++)
                {
                    var stripeRect = new Rect(rect.x, rect.y + (i * stripeHeight), rect.width, stripeHeight);
                    var col = layerColors[i];
                    col.a = 0.15f;
                    if (isHovered) col.a *= 1.3f;
                    EditorGUI.DrawRect(stripeRect, col);
                }
            }
        }
    }
    internal static class SceneLayerEditorSettings
    {
        private const string USE_COLORS_KEY = "SceneLayers.UseSceneColors";
        private const string CLICK_BEHAVIOR_KEY = "SceneLayers.ObjectRowClickBehavior";
        private const string BUTTON_STYLE_KEY = "SceneLayers.ButtonStyleIndex";
        private const string MULTI_LAYER_SPLIT_KEY = "SceneLayers.MultiLayerSplitMode";

        public enum ClickBehavior { Ping = 0, Select = 1 }
        public enum ButtonStyle { Boxed = 0, IconOnly = 1 }
        public enum MultiLayerSplit { Vertical = 0, Horizontal = 1 }

        public static bool UseSceneColors
        {
            get => EditorPrefs.GetBool(USE_COLORS_KEY, true);
            set => EditorPrefs.SetBool(USE_COLORS_KEY, value);
        }

        public static ClickBehavior ObjectRowClickBehavior
        {
            get => (ClickBehavior)EditorPrefs.GetInt(CLICK_BEHAVIOR_KEY, 0);
            set => EditorPrefs.SetInt(CLICK_BEHAVIOR_KEY, (int)value);
        }
        public static MultiLayerSplit MultiLayerSplitMode
        {
            get => (MultiLayerSplit)EditorPrefs.GetInt(MULTI_LAYER_SPLIT_KEY, 0);
            set => EditorPrefs.SetInt(MULTI_LAYER_SPLIT_KEY, (int)value);
        }
        public static int IconSizePx => 22;
    }

    public class SceneLayerOptionsWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            var win = GetWindow<SceneLayerOptionsWindow>();
            win.titleContent = new GUIContent("Scene Layer Options");
            win.minSize = new Vector2(340, 200);
            win.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Scene Layers – Options", EditorStyles.boldLabel);
            EditorGUILayout.Space(6);

            bool useColors = EditorGUILayout.Toggle("Layer Colors", SceneLayerEditorSettings.UseSceneColors);
            if (useColors != SceneLayerEditorSettings.UseSceneColors)
            {
                SceneLayerEditorSettings.UseSceneColors = useColors;
                RepaintAll();
            }

            var cbOpts = new[] { "Ping", "Select" };
            int cbIndex = EditorGUILayout.Popup("Clicking Object Row", (int)SceneLayerEditorSettings.ObjectRowClickBehavior, cbOpts);
            if (cbIndex != (int)SceneLayerEditorSettings.ObjectRowClickBehavior)
            {
                SceneLayerEditorSettings.ObjectRowClickBehavior = (SceneLayerEditorSettings.ClickBehavior)cbIndex;
            }

            EditorGUILayout.Space(4);
            GUILayout.Label("Multi-Layer Objects", EditorStyles.boldLabel);

            var splitOpts = new[] { "Vertical Stripes", "Horizontal Stripes" };
            int splitIndex = EditorGUILayout.Popup("Color Split Direction", (int)SceneLayerEditorSettings.MultiLayerSplitMode, splitOpts);
            if (splitIndex != (int)SceneLayerEditorSettings.MultiLayerSplitMode)
            {
                SceneLayerEditorSettings.MultiLayerSplitMode = (SceneLayerEditorSettings.MultiLayerSplit)splitIndex;
                RepaintAll();
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.HelpBox("Right-click a layer header or click its ⚙ to open rules. Drag scene objects or prefabs onto a layer to assign quickly.", MessageType.None);
        }

        private static void RepaintAll()
        {
            var mgr = GetWindow<SceneLayerManagerWindow>(false, null, false);
            if (mgr) mgr.Repaint();
            EditorApplication.RepaintHierarchyWindow();
            SceneView.RepaintAll();
        }
    }
    internal class PrefabDropPopup : PopupWindowContent
    {
        private readonly List<GameObject> _prefabs;
        private readonly SceneLayerDatabase.LayerDefinition _layer;
        private readonly SceneLayerDatabase _database;
        private bool _updateExisting = false;

        public PrefabDropPopup(IEnumerable<GameObject> prefabs,
                               SceneLayerDatabase.LayerDefinition layer,
                               SceneLayerDatabase database)
        {
            _prefabs = prefabs?.Distinct().Where(p => p).ToList() ?? new List<GameObject>();
            _layer = layer;
            _database = database;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("Drop Prefab onto Layer", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Layer:", _layer != null ? _layer.displayName : "(none)");

            if (_prefabs.Count == 1)
                EditorGUILayout.LabelField("Prefab:", _prefabs[0].name);
            else
                EditorGUILayout.LabelField("Prefabs:", _prefabs.Count.ToString());

            EditorGUILayout.Space(4);
            _updateExisting = EditorGUILayout.ToggleLeft("Also update existing instances in scene", _updateExisting);

            EditorGUILayout.Space(8);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Apply to Prefab Asset", GUILayout.MinWidth(140)))
                {
                    SceneLayerManagerWindow.ApplyLayerToPrefabAssets(_prefabs, _layer, _database, _updateExisting);
                    editorWindow.Close();
                }

                if (GUILayout.Button("Instantiate in Scene", GUILayout.MinWidth(140)))
                {
                    SceneLayerManagerWindow.InstantiatePrefabsInScene(_prefabs, _layer, _database);
                    editorWindow.Close();
                }
            }

            EditorGUILayout.Space(6);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Cancel", GUILayout.Width(90)))
                {
                    editorWindow.Close();
                }
                GUILayout.FlexibleSpace();
            }
        }
    }
    internal class AutoAssignRulesPopup : PopupWindowContent
    {
        private readonly SceneLayerDatabase _db;
        private readonly SceneLayerDatabase.LayerDefinition _layer;
        private readonly EditorWindow _owner;

        private Vector2 _scrollRules;
        private Vector2 _scrollResults;

        private string _search = "";
        private int _lastScanAdded = -1;

        private static List<Type> _allComponentTypes;

        private static readonly string[] kQuickPickTypeNames = new[]
        {
            "UnityEngine.Light",
            "UnityEngine.Camera",
            "UnityEngine.MeshRenderer",
            "UnityEngine.SpriteRenderer",
            "UnityEngine.AudioSource",
            "UnityEngine.Rigidbody",
            "UnityEngine.Rigidbody2D",
            "UnityEngine.BoxCollider",
            "UnityEngine.BoxCollider2D",
            "UnityEngine.Collider",
            "UnityEngine.Animator",
            "UnityEngine.ParticleSystem",
        };

        public AutoAssignRulesPopup(SceneLayerDatabase db, SceneLayerDatabase.LayerDefinition layer)
            : this(db, layer, null) { }

        public AutoAssignRulesPopup(SceneLayerDatabase db, SceneLayerDatabase.LayerDefinition layer, EditorWindow owner)
        {
            _db = db;
            _layer = layer;
            _owner = owner;
            EnsureAllComponentTypesCached();
        }

        public override Vector2 GetWindowSize() => new Vector2(520f, 700f);

        public override void OnGUI(Rect rect)
        {
            if (_db == null || _layer == null)
            {
                EditorGUILayout.HelpBox("Database or layer missing.", MessageType.Error);
                return;
            }
            bool isPlayMode = EditorApplication.isPlayingOrWillChangePlaymode;

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label($"Layer Settings – {_layer.displayName}", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("✕", GUILayout.Width(20), GUILayout.Height(16)))
                {
                    editorWindow.Close();
                }
            }
            if (isPlayMode)
            {
                EditorGUILayout.Space(2);
                EditorGUILayout.HelpBox("Layer modifications are disabled in Play Mode.", MessageType.Warning);
                EditorGUILayout.Space(2);
            }

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Layer Color:", GUILayout.Width(80));
                using (new EditorGUI.DisabledScope(isPlayMode))
                {
                    Color newColor = EditorGUILayout.ColorField(_layer.color, GUILayout.Width(60));
                    if (!isPlayMode && newColor != _layer.color)
                    {
                        Undo.RecordObject(_db, "Change Layer Color");
                        _layer.color = newColor;
                        EditorUtility.SetDirty(_db);
                        _owner?.Repaint();
                        EditorApplication.RepaintHierarchyWindow();
                    }
                }
            }

            EditorGUILayout.Space();

            GUILayout.Label($"Auto-Assign Rules – {_layer.displayName}", EditorStyles.boldLabel);
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Current Rules", EditorStyles.miniBoldLabel);

                if (_layer.autoRules == null) _layer.autoRules = new List<SceneLayerDatabase.LayerDefinition.AutoAssignRule>();

                float rulesHeight = 200f;
                _scrollRules = EditorGUILayout.BeginScrollView(_scrollRules, GUILayout.Height(rulesHeight));
                if (_layer.autoRules.Count == 0)
                {
                    EditorGUILayout.HelpBox("No rules yet. Add a Component type below to auto-assign any GameObject that has it.", MessageType.Info);
                }
                else
                {
                    for (int i = _layer.autoRules.Count - 1; i >= 0; i--)
                    {
                        var rule = _layer.autoRules[i];
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label(string.IsNullOrEmpty(rule.componentTypeName) ? "(missing type)" : rule.componentTypeName, GUILayout.ExpandWidth(true));
                            using (new EditorGUI.DisabledScope(isPlayMode))
                            {
                                if (GUILayout.Button("Remove", GUILayout.Width(70)))
                                {
                                    if (!isPlayMode)
                                    {
                                        Undo.RecordObject(_db, "Remove Auto-Assign Rule");
                                        _layer.autoRules.RemoveAt(i);
                                        EditorUtility.SetDirty(_db);
                                        _owner?.Repaint();
                                    }
                                }
                            }
                        }
                    }
                }
                EditorGUILayout.EndScrollView();
            }
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Quick Picks (Unity Components)", EditorStyles.miniBoldLabel);
                    DrawQuickPickGrid(kQuickPickTypeNames, 3, tName =>
                    {
                        if (!isPlayMode) TryAddRuleForTypeName(tName);
                    });
                }
            }

            GUILayout.Space(2);
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    GUILayout.Label("Browse / Search All Component Types", EditorStyles.miniBoldLabel);
                    using (new GUILayout.HorizontalScope())
                    {
                        _search = EditorGUILayout.TextField(_search, GUI.skin.FindStyle("SearchTextField"));
                        if (GUILayout.Button("Clear", GUILayout.Width(60))) _search = string.Empty;
                    }

                    var filtered = FilterTypes(_allComponentTypes, _search);

                    using (new GUILayout.HorizontalScope())
                    {
                        EditorGUILayout.LabelField($"{filtered.Count} match(es)", EditorStyles.miniLabel, GUILayout.Width(120));
                        GUILayout.FlexibleSpace();
                    }

                    float searchResultsHeight = Mathf.Min(120f, editorWindow.position.height - 320f);
                    _scrollResults = EditorGUILayout.BeginScrollView(_scrollResults, GUILayout.Height(searchResultsHeight));
                    foreach (var t in filtered)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.Label(t.FullName, GUILayout.Width(380));
                            bool has = HasRuleForType(t);
                            using (new EditorGUI.DisabledScope(has || isPlayMode))
                            {
                                if (GUILayout.Button(has ? "Added" : "Add", GUILayout.Width(64)))
                                {
                                    if (!isPlayMode) TryAddRuleForType(t);
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
            }

            GUILayout.Space(6);
            using (new EditorGUI.DisabledScope(isPlayMode))
            {
                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.FlexibleSpace();
                    string scanTooltip = isPlayMode ? "Scan Scene Now (disabled in Play Mode)" : "Scan Scene Now";
                    if (GUILayout.Button(new GUIContent(scanTooltip), GUILayout.Width(160), GUILayout.Height(24)))
                    {
                        if (!isPlayMode)
                        {
                            _lastScanAdded = SceneLayerManagerWindow.ScanSceneForLayerRules(_db, _layer);
                        }
                    }
                    GUILayout.FlexibleSpace();
                }
            }

            if (_lastScanAdded >= 0)
            {
                EditorGUILayout.HelpBox($"{_lastScanAdded} object(s) added to layer \"{_layer.displayName}\".", MessageType.Info);
            }
            else
            {
                var wrapStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    wordWrap = true
                };

                string helpText = isPlayMode
                    ? "Layer modifications are disabled in Play Mode. Exit Play Mode to use auto-assign rules."
                    : "Auto-assign works on new objects/components only. Use \"Scan Scene Now\" to add existing objects that match the rules.";

                EditorGUILayout.LabelField(helpText, wrapStyle);
            }
        }

        private static void EnsureAllComponentTypesCached()
        {
            if (_allComponentTypes != null) return;

            var list = new List<Type>(TypeCache.GetTypesDerivedFrom<Component>());
            list.RemoveAll(t =>
                t == null ||
                t.IsAbstract ||
                t.IsGenericType ||
                t.FullName == null);

            list.Sort((a, b) => string.CompareOrdinal(a.FullName, b.FullName));
            _allComponentTypes = list;
        }

        private bool HasRuleForType(Type t)
        {
            if (t == null || _layer.autoRules == null) return false;
            string name = t.FullName;
            return _layer.autoRules.Any(r => r.componentTypeName == name);
        }

        private void TryAddRuleForType(Type t)
        {
            if (t == null)
            {
                Debug.LogWarning("Cannot add rule for null type");
                return;
            }

            if (!typeof(Component).IsAssignableFrom(t))
            {
                Debug.LogWarning($"Type {t.Name} is not a Component");
                return;
            }

            if (_layer.autoRules == null)
                _layer.autoRules = new List<SceneLayerDatabase.LayerDefinition.AutoAssignRule>();

            if (HasRuleForType(t))
            {
                Debug.Log($"Rule for {t.Name} already exists");
                return;
            }

            Undo.RecordObject(_db, "Add Auto-Assign Rule");
            _layer.autoRules.Add(new SceneLayerDatabase.LayerDefinition.AutoAssignRule
            {
                componentTypeName = t.FullName
            });
            EditorUtility.SetDirty(_db);
            _owner?.Repaint();
            editorWindow?.Repaint();

            Debug.Log($"Added rule for {t.Name}");
        }

        private void TryAddRuleForTypeName(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                Debug.LogWarning("Cannot add rule for empty type name");
                return;
            }
            Type t = ResolveTypeByName(typeName);

            if (t != null)
            {
                TryAddRuleForType(t);
            }
            else
            {
                Debug.LogWarning($"Could not resolve type: {typeName}");
            }
        }

        private static Type ResolveTypeByName(string typeName)
        {
            Type t = Type.GetType(typeName);
            if (t != null && typeof(Component).IsAssignableFrom(t))
                return t;
            var unityEngineAssembly = typeof(GameObject).Assembly;
            t = unityEngineAssembly.GetType(typeName);
            if (t != null && typeof(Component).IsAssignableFrom(t))
                return t;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    t = assembly.GetType(typeName);
                    if (t != null && typeof(Component).IsAssignableFrom(t))
                        return t;
                }
                catch (System.Exception)
                {
                    continue;
                }
            }
            string simpleName = typeName.Contains(".") ? typeName.Substring(typeName.LastIndexOf('.') + 1) : typeName;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    var types = assembly.GetTypes();
                    t = types.FirstOrDefault(type =>
                        type.Name == simpleName &&
                        typeof(Component).IsAssignableFrom(type));
                    if (t != null)
                        return t;
                }
                catch (System.Exception)
                {
                    continue;
                }
            }

            return null;
        }

        private static List<Type> FilterTypes(List<Type> source, string search)
        {
            if (source == null) return new List<Type>();
            if (string.IsNullOrWhiteSpace(search)) return source;

            search = search.Trim();
            var tokens = search.Split(' ').Where(t => !string.IsNullOrWhiteSpace(t)).ToArray();
            return source.Where(t =>
            {
                var n = t.FullName;
                foreach (var tok in tokens)
                    if (n.IndexOf(tok, StringComparison.OrdinalIgnoreCase) < 0)
                        return false;
                return true;
            }).ToList();
        }

        private static void DrawQuickPickGrid(string[] typeNames, int perRow, Action<string> onClick)
        {
            if (typeNames == null || typeNames.Length == 0) return;
            int i = 0;
            while (i < typeNames.Length)
            {
                EditorGUILayout.BeginHorizontal();
                for (int c = 0; c < perRow && i < typeNames.Length; c++, i++)
                {
                    string full = typeNames[i];
                    string label = ShortTypeName(full);
                    if (GUILayout.Button(label, GUILayout.MinWidth(140)))
                    {
                        onClick?.Invoke(full);
                        GUI.changed = true;
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private static string ShortTypeName(string fullName)
        {
            if (string.IsNullOrEmpty(fullName)) return "(null)";
            int lastDot = fullName.LastIndexOf('.');
            return lastDot >= 0 ? fullName.Substring(lastDot + 1) : fullName;
        }
    }

    internal class SimpleTextPopup : PopupWindowContent
    {
        private readonly string _title;
        private readonly string _label;
        private readonly System.Action<string, Color> _onSubmit;
        private string _text = string.Empty;
        private Color _selectedColor;

        public SimpleTextPopup(string title, string label, string prefillText, System.Action<string, Color> onSubmit)
        {
            _title = title;
            _label = label;
            _onSubmit = onSubmit;
            _text = prefillText ?? string.Empty;
            _selectedColor = SceneLayerManagerWindow.PeekNextDefaultColor();
        }

        public SimpleTextPopup(string title, string label, string prefillText, System.Action<string> onSubmit)
            : this(title, label, prefillText, (text, color) => onSubmit?.Invoke(text))
        {
            _selectedColor = Color.white;
        }

        public override Vector2 GetWindowSize() => new Vector2(320, 90);

        public override void OnGUI(Rect rect)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(_title, EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("✕", GUILayout.Width(20), GUILayout.Height(16)))
                {
                    editorWindow.Close();
                }
            }

            EditorGUILayout.Space(4);
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label(_label, GUILayout.Width(80));

                GUI.SetNextControlName("PopupTextField");
                _text = EditorGUILayout.TextField(_text, GUILayout.Width(160));

                GUILayout.Space(5);
                _selectedColor = EditorGUILayout.ColorField(GUIContent.none, _selectedColor,
                    false, false, false, GUILayout.Width(50));
            }

            if (Event.current.type == EventType.Repaint && GUI.GetNameOfFocusedControl() != "PopupTextField")
            {
                GUI.FocusControl("PopupTextField");
                EditorApplication.delayCall += () =>
                {
                    var textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), GUIUtility.keyboardControl);
                    if (textEditor != null) textEditor.SelectAll();
                };
            }

            if (Event.current.type == EventType.KeyDown &&
                (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter))
            {
                CreateAction();
                Event.current.Use();
            }

            EditorGUILayout.Space(6);
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Create", GUILayout.Width(140), GUILayout.Height(24)))
                {
                    CreateAction();
                }
                GUILayout.FlexibleSpace();
            }
            EditorGUILayout.Space(2);
        }

        private void CreateAction()
        {
            if (!string.IsNullOrWhiteSpace(_text))
            {
                SceneLayerManagerWindow.IncrementDefaultColorIndex();
            }
            _onSubmit?.Invoke(_text, _selectedColor);
            editorWindow.Close();
        }
    }
}
#endif
