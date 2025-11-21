#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SceneLayers.Editor
{
    internal static class SceneLayerController
    {
        public static IEnumerable<GameObject> EnumerateAllSceneObjects()
        {
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;
                var roots = scene.GetRootGameObjects();
                foreach (var root in roots)
                {
                    foreach (var t in root.GetComponentsInChildren<Transform>(true))
                        yield return t.gameObject;
                }
            }
        }

        public static IEnumerable<GameObject> GetObjectsInLayer(string layerGuid, SceneLayerDatabase database)
        {
            SceneLayerManagerWindow.RebuildCacheIfNeeded();

            if (SceneLayerManagerWindow.m_layerObjectCache.TryGetValue(layerGuid, out var objects))
            {
                foreach (var go in objects)
                {
                    if (go != null)
                        yield return go;
                }
            }
        }

        public static int CountObjectsInLayer(string layerGuid, SceneLayerDatabase database)
        {
            SceneLayerManagerWindow.RebuildCacheIfNeeded();

            if (SceneLayerManagerWindow.m_layerObjectCache.TryGetValue(layerGuid, out var objects))
            {
                return objects.Count(go => go != null);
            }

            return 0;
        }

        public static void AssignSelectionToLayer(SceneLayerDatabase db, SceneLayerDatabase.LayerDefinition layer)
        {
            if (db == null || layer == null) return;

            Undo.RecordObject(db, "Assign Layer");

            foreach (var go in Selection.gameObjects)
            {
                if (!go.scene.IsValid()) continue;

                string goid = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();

                if (layer.objectGlobalIds == null)
                    layer.objectGlobalIds = new List<string>();

                if (!layer.objectGlobalIds.Contains(goid))
                {
                    layer.objectGlobalIds.Add(goid);
                }
            }

            EditorUtility.SetDirty(db);
        }

        public static void RemoveSelectionFromLayer(SceneLayerDatabase db, SceneLayerDatabase.LayerDefinition layer)
        {
            if (db == null || layer == null) return;

            Undo.RecordObject(db, "Remove Layer");

            foreach (var go in Selection.gameObjects)
            {
                if (!go.scene.IsValid()) continue;

                string goid = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();

                if (layer.objectGlobalIds != null)
                {
                    layer.objectGlobalIds.Remove(goid);
                }
            }

            EditorUtility.SetDirty(db);
        }

        public static void SetLayerVisibility(SceneLayerDatabase db, SceneLayerDatabase.LayerDefinition layer, bool visible)
        {
            SceneLayerManagerWindow.RebuildCacheIfNeeded();

            if (!SceneLayerManagerWindow.m_layerObjectCache.TryGetValue(layer.guid, out var objectSet))
                return;

            var objs = objectSet.Where(go => go != null).ToArray();
            if (objs.Length == 0) return;

            var svm = SceneVisibilityManager.instance;
            if (visible)
                svm.Show(objs, true);
            else
                svm.Hide(objs, true);
            EditorApplication.delayCall += () =>
            {
                SceneLayerManagerWindow.ClearVisibilityCacheForObjects(objs);
            };
        }

        public static void SetLayerPickable(SceneLayerDatabase db, SceneLayerDatabase.LayerDefinition layer, bool pickable)
        {
            SceneLayerManagerWindow.RebuildCacheIfNeeded();

            if (!SceneLayerManagerWindow.m_layerObjectCache.TryGetValue(layer.guid, out var objectSet))
                return;

            var objs = objectSet.Where(go => go != null).ToArray();
            if (objs.Length == 0) return;

            var svm = SceneVisibilityManager.instance;
            if (pickable)
                svm.EnablePicking(objs, true);
            else
                svm.DisablePicking(objs, true);
            EditorApplication.delayCall += () =>
            {
                SceneLayerManagerWindow.ClearVisibilityCacheForObjects(objs);
            };
        }

        public static List<string> GetLayersForObject(GameObject go, SceneLayerDatabase database)
        {
            if (database == null || go == null) return new List<string>();

            string goid = GlobalObjectId.GetGlobalObjectIdSlow(go).ToString();
            var layers = new List<string>();

            foreach (var layer in database.layers)
            {
                if (layer.objectGlobalIds != null && layer.objectGlobalIds.Contains(goid))
                {
                    layers.Add(layer.guid);
                }
            }

            return layers;
        }
    }
}
#endif