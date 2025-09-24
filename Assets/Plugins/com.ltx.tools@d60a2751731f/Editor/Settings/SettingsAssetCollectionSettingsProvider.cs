using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LTX.Editor;
using LTX.Tools.Settings.Attributes;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace LTX.Tools.Settings
{
    public static class SettingsAssetCollectionSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateMainSettingsProvider()
        {
            var provider = new SettingsProvider("LTX", SettingsScope.Project)
            {
                activateHandler = (s, element) =>
                {
                    SettingsAssetCollectionSettings instance = SettingsAssetCollectionSettings.instance;
                    SerializedObject serializedObject = new SerializedObject(instance);
                    SerializedProperty assetProperty = serializedObject.FindBackingFieldProperty(
                        nameof(SettingsAssetCollectionSettings.Collection));

                    PropertyField path = new PropertyField(assetProperty, "Project Asset");
                    path.RegisterValueChangeCallback(ctx => instance.Save());

                    VisualElement inner = new VisualElement()
                    {
                        name = "LTX settings container"
                    };

                    inner.StyleSettingsContentWithTitle("Global Settings", new Color(0.2f, 0.54f, 0.8f));

                    inner.Add(path);
                    inner.Bind(serializedObject);

                    element.Add(inner);
                }
            };

            return provider;
        }

        [SettingsProviderGroup]

        public static SettingsProvider[] CreateOtherSettingsProvider()
        {
            SettingsCollection collection = SettingsAssetCollectionSettings.instance.GetCollection();

            return collection.Assets.Select(scriptableObject =>
            {
                Type type = scriptableObject.GetType();
                LTXSettingsTitleAttribute att = type.GetCustomAttribute(typeof(LTXSettingsTitleAttribute)) as LTXSettingsTitleAttribute;

                string title = att?.title?? scriptableObject.name;
                Color color = att?.color ?? LTXSettingsTitleAttribute.DefaultColor;

                return new SettingsProvider($"LTX/{title}", SettingsScope.Project)
                {
                    activateHandler = (s, element) =>
                    {
                        VisualElement inner = new VisualElement()
                        {
                            name = "LTX settings container"
                        };
                        inner.StyleSettingsContentWithTitle(title, color);

                        InspectorElement inspectorElement = new InspectorElement(scriptableObject);
                        inner.Add(inspectorElement);

                        element.Add(inner);
                    }
                };
            }).ToArray();

        }
    }
}