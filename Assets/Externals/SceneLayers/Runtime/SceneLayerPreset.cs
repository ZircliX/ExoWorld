using UnityEngine;
using System.Collections.Generic;

namespace SceneLayers
{
    [System.Serializable]
    public class SceneLayerPreset : ScriptableObject
    {
        public string presetName = "New Preset";
        public List<LayerDefinitionData> layers = new List<LayerDefinitionData>();
        public List<LayerViewData> views = new List<LayerViewData>();

        [System.Serializable]
        public class LayerDefinitionData
        {
            public string displayName;
            public Color color = Color.white;
            public bool defaultVisible = true;
            public bool defaultPickable = true;
            public bool isExpanded = false;
            public List<AutoAssignRuleData> autoRules = new List<AutoAssignRuleData>();
        }

        [System.Serializable]
        public class AutoAssignRuleData
        {
            public string componentTypeName;
        }

        [System.Serializable]
        public class LayerViewData
        {
            public string viewName;
            public Color color;
            public List<LayerStateData> states = new List<LayerStateData>();
        }

        [System.Serializable]
        public class LayerStateData
        {
            public string layerDisplayName;
            public bool visible = true;
            public bool pickable = true;
            public bool expanded = false;
        }
    }
}