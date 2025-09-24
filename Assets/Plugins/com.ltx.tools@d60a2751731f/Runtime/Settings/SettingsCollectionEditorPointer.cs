#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;

namespace LTX.Tools.Settings
{
    public class SettingsCollectionEditorPointer : ScriptableSingleton<SettingsCollectionEditorPointer>
    {
        [SerializeField]
        public SettingsCollection collection;

        private void OnEnable()
        {
            FindCollection();
        }


        [InitializeOnLoadMethod]
        private static void Init()
        {
            if (instance.collection == null)
            {
                instance.FindCollection();
            }
        }

        private void FindCollection()
        {
            var assets = PlayerSettings.GetPreloadedAssets();
            for (int i = 0; i < assets.Length; i++)
            {
                if (assets[i] is SettingsCollection c)
                {
                    collection = c;
                    break;
                }
            }
        }
    }
}
#endif