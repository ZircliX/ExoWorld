using LTX.Tools.Editor.Annotations.UIToolkit;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor.Annotations.Drawers
{
    [CustomPropertyDrawer(typeof(Annotable), false), System.Serializable]
    public class AnnotablePropertyDrawer : PropertyDrawer
    {
        protected virtual string VisualAssetTreePath => "Packages/com.ltx.tools/Editor/UIToolkit/AnnotableUXML.uxml";

        private const string PANEL_ENABLE_CLASS = "panel-enable";
        private const string PANEL_DISABLE_CLASS = "panel-disable";


        private VisualTreeAsset visualTreeAsset;

        private VisualElement editPanel;
        private VisualElement showPanel;

        private Slider textSizeSlider;
        private HelpBox helpBox;
        private TextField textField;
        private ToolbarButton saveButton;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return new AnnotableElement(property);
        }
    }
}