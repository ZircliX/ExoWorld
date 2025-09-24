#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

namespace LTX.Tools.Settings
{
    public partial class SettingsCollection
    {
        private static bool IsPlaying;

        [InitializeOnLoadMethod]
        private static void SubToPlayModeChange()
        {
            EditorApplication.playModeStateChanged += change =>
            {
                IsPlaying = change switch
                {
                    PlayModeStateChange.EnteredEditMode => false,
                    PlayModeStateChange.ExitingEditMode => true,
                    PlayModeStateChange.EnteredPlayMode => true,
                    PlayModeStateChange.ExitingPlayMode => false,
                    _ => false
                };
            };
        }
    }
}
#endif