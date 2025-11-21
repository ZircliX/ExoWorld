using System;
using System.Collections.Generic;
using UnityEngine;

namespace SceneLayers
{
    [CreateAssetMenu(menuName = "Scene Layers/Layer Database", fileName = "SceneLayerDatabase")]
    public class SceneLayerDatabase : ScriptableObject
    {
        [Serializable]
        public class LayerDefinition
        {
            public string guid = Guid.NewGuid().ToString();
            public string displayName = "New Layer";
            public Color color = new Color(0.5f, 0.8f, 1f, 1f);
            [SerializeField]
            public List<string> objectGlobalIds = new List<string>();
            [System.Serializable]
            public class AutoAssignRule
            {
                public string componentTypeName;
            }
            public List<AutoAssignRule> autoRules = new List<AutoAssignRule>();
            public bool autoAssignLive = false;
            public bool defaultVisible = true;
            public bool defaultPickable = true;
            public List<string> objectOrder = new List<string>();
        }

        [Serializable]
        public class LayerView
        {
            public string viewName = "New View";
            public Color color = new Color(0.3f, 0.50f, 0.7f, 1f);
            public List<LayerState> states = new List<LayerState>();
        }

        [Serializable]
        public class LayerState
        {
            public string layerGuid;
            public bool visible = true;
            public bool pickable = true;
            public bool expanded = false;
        }

        public List<LayerDefinition> layers = new List<LayerDefinition>();
        public List<LayerView> views = new List<LayerView>();

        public LayerDefinition GetLayer(string guid)
            => layers.Find(l => l.guid == guid);

        public LayerDefinition GetOrCreate(string name)
        {
            var existing = layers.Find(l => l.displayName == name);
            if (existing != null) return existing;
            var def = new LayerDefinition { displayName = name };
            layers.Add(def);
            return def;
        }
    }
}