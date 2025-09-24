using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor.Annotations.UIToolkit
{
    public class BaseAnnotableElement : BindableElement, INotifyValueChanged<string>
    {
        private const string PANEL_ENABLE_CLASS = "panel-enable";
        private const string PANEL_DISABLE_CLASS = "panel-disable";

        private readonly VisualElement editPanel;
        private readonly VisualElement showPanel;

        private readonly Slider textSizeSlider;
        private readonly HelpBox helpBox;
        private readonly TextField textField;
        private readonly ToolbarButton saveButton;

        private string mValue;

        public string value
        {
            get => mValue;
            set
            {
                if (EqualityComparer<string>.Default.Equals(mValue, value))
                    return;

                helpBox.text = value;

                if (panel != null)
                {
                    using ChangeEvent<string> evt = ChangeEvent<string>.GetPooled(mValue, value);

                    evt.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(evt);
                }
                else
                    SetValueWithoutNotify(value);
            }
        }

        public void SetValueWithoutNotify(string newValue)
        {
            mValue = newValue;
        }

        public BaseAnnotableElement(SerializedProperty property, string uxmlPath) : this(uxmlPath)
        {
            this.BindProperty(property);
            var propertyPath = textField.bindingPath;

            value = property.FindPropertyRelative(propertyPath).stringValue;
        }


        public BaseAnnotableElement(string uxmlPath)
        {
            VisualTreeAsset visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);

            VisualElement root = visualTreeAsset.Instantiate();
            Add(root);

            editPanel = root.Query("EditMode").First();
            showPanel = root.Query("DisplayMode").First();

            //Slider
            textSizeSlider = root.Q<Slider>("TextSizeSlider");

            textSizeSlider.RegisterCallback<ChangeEvent<float>>((evt) =>
            {
                helpBox.Q<Label>().style.fontSize = evt.newValue;
                if (textField != null)
                    textField.style.fontSize = evt.newValue;
            });

            //Help box
            helpBox = root.Q<HelpBox>();
            helpBox.AddManipulator(new Clickable(Edit));

            //Text field
            textField = root.Q<TextField>("InputField");
            textField.RegisterCallback(new EventCallback<FocusOutEvent>(_ => Save()));

            saveButton = root.Q<ToolbarButton>("Save");
            saveButton.clicked += Save;
            mValue = string.Empty;
            textField.value = string.Empty;
            helpBox.text = string.Empty;

            Show();
        }


        private void Save()
        {
            Show();
            value = textField.value;
        }


        public void Edit()
        {
            editPanel.RemoveFromClassList(PANEL_DISABLE_CLASS);
            editPanel.AddToClassList(PANEL_ENABLE_CLASS);

            showPanel.RemoveFromClassList(PANEL_ENABLE_CLASS);
            showPanel.AddToClassList(PANEL_DISABLE_CLASS);

            saveButton.RemoveFromClassList(PANEL_DISABLE_CLASS);
            saveButton.AddToClassList(PANEL_ENABLE_CLASS);
        }

        public void Show()
        {
            showPanel?.RemoveFromClassList(PANEL_DISABLE_CLASS);
            showPanel?.AddToClassList(PANEL_ENABLE_CLASS);

            editPanel?.RemoveFromClassList(PANEL_ENABLE_CLASS);
            editPanel?.AddToClassList(PANEL_DISABLE_CLASS);

            saveButton.RemoveFromClassList(PANEL_ENABLE_CLASS);
            saveButton.AddToClassList(PANEL_DISABLE_CLASS);
        }

    }
}