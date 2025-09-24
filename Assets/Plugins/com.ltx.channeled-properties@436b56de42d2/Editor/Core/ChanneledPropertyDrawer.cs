using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace LTX.ChanneledProperties.Editor.Core
{
    public abstract class ChanneledPropertyDrawer : PropertyDrawer
    {
        private const int BorderRadius = 4;


        private static StyleSheet StyleSheet
        {
            get
            {
                if(styleSheet == null)
                    styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.ltx.channeled-properties/Editor/ChanneledPropertyDrawer.uss");

                return styleSheet;
            }
        }
        private static StyleSheet styleSheet;

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            VisualElement container = new VisualElement()
            {
                style =
                {
                    marginTop = 2,
                    marginBottom = 2,
                    paddingBottom = 1,
                    paddingTop = 1,
                    paddingLeft = 3,
                    paddingRight = 3,
                    backgroundColor = new Color(.15f, .15f, .15f),
                    borderBottomLeftRadius = BorderRadius,
                    borderBottomRightRadius = BorderRadius,
                    borderTopLeftRadius = BorderRadius,
                    borderTopRightRadius = BorderRadius,
                }
            };
            container.styleSheets.Add(StyleSheet);

            Label label = new Label(property.displayName)
            {
                style =
                {
                    unityTextAlign = TextAnchor.MiddleCenter,
                    paddingTop = 5,
                    paddingBottom = 5,
                }
            };

            container.Add(label);

            if (Application.isPlaying)
            {

                Object targetObject = property.serializedObject.targetObject;
                object channeledProperty = fieldInfo.GetValue(targetObject);
                Type type = channeledProperty.GetType();

                while (type != null && type.BaseType != typeof(object))
                {
                    type = type.BaseType;
                }

                if (type == null)
                    return null;

                Type valueType = type.GetGenericArguments()[0];

                var getElementMethodInfo = typeof(ChanneledPropertyDrawer).GetMethod(nameof(GetElement),
                    BindingFlags.Instance | BindingFlags.NonPublic);
                if (getElementMethodInfo == null)
                    return null;

                var genericMethod = getElementMethodInfo.MakeGenericMethod(valueType);
                var result = genericMethod.Invoke(this, new object[] { property, channeledProperty });
                if (result is VisualElement visualElement)
                {
                    if (result is IChanneledPropertyElement element)
                    {
                        visualElement.RegisterCallback<AttachToPanelEvent>(evt => element.Draw());
                        visualElement.RegisterCallback<DetachFromPanelEvent>(evt => element.Dispose());
                    }
                    container.Add(visualElement);

                    label.tooltip = "Click to collapse or open infos";
                    label.RegisterCallback<ClickEvent>(ctx =>
                    {
                        if(visualElement.style.display == DisplayStyle.None)
                            visualElement.style.display = DisplayStyle.Flex;
                        else
                            visualElement.style.display = DisplayStyle.None;

                    });
                }
                container.Add(result as VisualElement);
            }
            else
            {

                HelpBox helpBox = new HelpBox("Channeled properties can only be viewed in playmode",
                    HelpBoxMessageType.Info);

                label.tooltip = "Click to collapse or open infos";
                label.RegisterCallback<ClickEvent>(ctx =>
                {
                    if(helpBox.style.display == DisplayStyle.None)
                        helpBox.style.display = DisplayStyle.Flex;
                    else
                        helpBox.style.display = DisplayStyle.None;
                });
                container.Add(helpBox);
            }

            return container;
        }

        protected abstract ChanneledPropertyElement<TValue> GetElement<TValue>(SerializedProperty property, IChanneledPropertyEditor<TValue> channeledProperty);
    }

}