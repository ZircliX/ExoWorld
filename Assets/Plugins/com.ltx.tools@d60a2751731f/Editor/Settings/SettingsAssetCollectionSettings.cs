using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;
using Object = UnityEngine.Object;

namespace LTX.Tools.Settings
{
    [FilePath("ProjectSettings/LTX/SettingsAssetCollectionSettings.Asset", FilePathAttribute.Location.ProjectFolder)]
    public class SettingsAssetCollectionSettings : ScriptableSingleton<SettingsAssetCollectionSettings>
    {

        [field: SerializeField]
        public SettingsCollection Collection { get; private set; }

        public SettingsCollection GetCollection()
        {
            if (Collection == null)
                CreateOrFindCollection();

            return Collection;
        }

        [InitializeOnLoadMethod]
        private static void Load()
        {
            try
            {
                TypeCache.TypeCollection typesDerivedFrom = TypeCache.GetTypesDerivedFrom(typeof(LTXSettingsAsset<>));
                instance.Setup(typesDerivedFrom);

                AssetDatabase.SaveAssets();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void Setup(TypeCache.TypeCollection types)
        {
            CreateOrFindCollection();

            SettingsCollection collection = GetCollection();

            CreateOrDeleteEmbedSettings(types, collection);
            UpdateSerializedObject(collection);

            SetAsPreloadedAsset(collection);
            Save(true);
        }

        private void UpdateSerializedObject(SettingsCollection collection)
        {
            using (SerializedObject serializedObject = new SerializedObject(collection))
            {
                string path = AssetDatabase.GetAssetPath(collection);
                Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);
                SerializedProperty property = serializedObject.FindProperty("settingsAssets");
                property.arraySize = subAssets.Length;
                for (int i = 0; i < subAssets.Length; i++)
                    property.GetArrayElementAtIndex(i).objectReferenceValue = subAssets[i];

                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private void CreateOrDeleteEmbedSettings(TypeCache.TypeCollection types, SettingsCollection collection)
        {
            string path = AssetDatabase.GetAssetPath(collection);
            Object[] subAssets = AssetDatabase.LoadAllAssetRepresentationsAtPath(path);

            Dictionary<Type, Object> existingSettings = new();
            for (int i = 0; i < subAssets.Length; i++)
            {
                Object subAsset = subAssets[i];
                if(subAsset == null)
                    continue;

                Type t = subAsset.GetType();
                if (!existingSettings.TryAdd(t, subAsset))
                    AssetDatabase.RemoveObjectFromAsset(subAsset);
            }

            foreach (Type type in types)
            {
                if (!existingSettings.Remove(type))
                {
                    ScriptableObject scriptableObject = CreateInstance(type);
                    scriptableObject.name = type.Name;
                    AssetDatabase.AddObjectToAsset(scriptableObject, collection);
                }
            }

            foreach (var obj in existingSettings.Values)
                AssetDatabase.RemoveObjectFromAsset(obj);
        }

        private static void SetAsPreloadedAsset(SettingsCollection collection)
        {
            Object[] preloadedAssets = PlayerSettings.GetPreloadedAssets();
            if (preloadedAssets.All(ctx => ctx != collection))
            {
                PlayerSettings.SetPreloadedAssets(
                    new List<Object>(preloadedAssets.Where(ctx => ctx!=null))
                    {
                        collection
                    }.ToArray()
                );
            }
        }

        internal void Save() => Save(true);

        internal void CreateOrFindCollection()
        {
            string[] assets = AssetDatabase.FindAssets($"t:{nameof(SettingsCollection)}");

            if (assets.Length == 0)
            {
                Collection = CreateInstance<SettingsCollection>();
                Collection.hideFlags = HideFlags.NotEditable;

                AssetDatabase.CreateAsset(Collection, "Assets/LTXSettings.asset");
            }
            else
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(assets[0]);
                Collection = AssetDatabase.LoadAssetAtPath<SettingsCollection>(assetPath);
                Collection.hideFlags = HideFlags.NotEditable;
            }

        }
    }
}