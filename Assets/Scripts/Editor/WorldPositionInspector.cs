using OverBang.GameName.Core;
using UnityEditor;
using UnityEngine;

namespace OverBang.GameName.Editor
{
    [CustomEditor(typeof(WorldPositionDisplay))]
    public class WorldPositionInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // Draw the normal inspector first
            //DrawDefaultInspector();

            Transform t = ((WorldPositionDisplay)target).transform;

            //EditorGUILayout.LabelField("Transform (World)", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            Vector3 newPos = EditorGUILayout.Vector3Field("World Position", t.position);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(t, "Change World Position");
                t.position = newPos;
                EditorUtility.SetDirty(t);
            }
        }
    }
}