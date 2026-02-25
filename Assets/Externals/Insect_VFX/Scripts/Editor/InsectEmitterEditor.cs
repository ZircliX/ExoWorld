using UnityEditor;
using UnityEngine;

namespace Insect_VFX
{
#if UNITY_EDITOR

    [CustomEditor(typeof(InsectEmitter))]
    [CanEditMultipleObjects]
    public class InsectEmitterEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            InsectEmitter emitter = (InsectEmitter)target;

            SerializedProperty loopMode = serializedObject.FindProperty("loopMode");
            SerializedProperty playOnAwake = serializedObject.FindProperty("playOnAwake");
            SerializedProperty numberOfEmissions = serializedObject.FindProperty("numberOfEmissions");
            SerializedProperty emissionObjects = serializedObject.FindProperty("emissionObjects");
            SerializedProperty simulationSize = serializedObject.FindProperty("simulationSize");
            SerializedProperty simulationHeight = serializedObject.FindProperty("simulationHeight");
            SerializedProperty navigationTexture = serializedObject.FindProperty("navigationTexture");
            SerializedProperty bakeResolutions = serializedObject.FindProperty("bakeResolutions");

            DrawHeader();
            
            EditorGUILayout.Space();
            DrawSectionTitle("Emission Settings");
            DrawSection(() =>
            {
                EditorGUILayout.PropertyField(playOnAwake);
                EditorGUILayout.PropertyField(loopMode);
                GUILayout.Space(8);
                EditorGUILayout.PropertyField(numberOfEmissions);
                EditorGUILayout.PropertyField(simulationSize);
                EditorGUILayout.PropertyField(simulationHeight);
                GUILayout.Space(8);
                EditorGUILayout.PropertyField(emissionObjects, true);
            });

            EditorGUILayout.Space();
            DrawSectionTitle("Entities Settings");
            DrawSection(() =>
            {
                SerializedProperty settingsPreset = serializedObject.FindProperty("settingsPreset");
                SerializedProperty moveSpeed = serializedObject.FindProperty("moveSpeed");
                SerializedProperty rotationSpeed = serializedObject.FindProperty("rotationSpeed");
                SerializedProperty pauseTime = serializedObject.FindProperty("pauseTime");
                SerializedProperty sizeVariation = serializedObject.FindProperty("sizeVariation");

                EditorGUILayout.PropertyField(settingsPreset);
                EditorGUILayout.PropertyField(moveSpeed);
                EditorGUILayout.PropertyField(rotationSpeed);
                EditorGUILayout.PropertyField(pauseTime);
                EditorGUILayout.PropertyField(sizeVariation);

                foreach (var obj in serializedObject.targetObjects)
                {
                    InsectEmitter emitterObj = (InsectEmitter)obj;
                    switch (emitterObj.settingsPreset)
                    {
                        case SettingsPreset.Default:
                            emitterObj.SetEntitiesSettings(0.75f, 15f, 2f);
                            break;
                    }
                }
            });

            EditorGUILayout.Space();
            DrawSectionTitle("PathFinding Settings");
            
            DrawSection(() =>
            {
                SerializedProperty surface_LayerMask = serializedObject.FindProperty("surface_LayerMask");
                SerializedProperty simulationMode = serializedObject.FindProperty("simulationMode");
                SerializedProperty pathfindingResolution = serializedObject.FindProperty("pathfindingResolution");

                EditorGUILayout.PropertyField(surface_LayerMask);
                EditorGUILayout.PropertyField(simulationMode);
                EditorGUILayout.PropertyField(pathfindingResolution);

                foreach (var obj in serializedObject.targetObjects)
                {
                    InsectEmitter emitterObj = (InsectEmitter)obj;
                    // Actualiza las configuraciones según la calidad de resolución
                    switch (emitterObj.pathfindingResolution)
                    {
                        case PathFindingQuality.Low:
                            emitterObj.pathfindingResolutionSteps = 5;
                            break;
                        case PathFindingQuality.Medium:
                            emitterObj.pathfindingResolutionSteps = 15;
                            break;
                        case PathFindingQuality.High:
                            emitterObj.pathfindingResolutionSteps = 30;
                            break;
                        case PathFindingQuality.MaxQuality:
                            emitterObj.pathfindingResolutionSteps = 50;
                            break;
                    }
                }
            });

            if(emitter.simulationMode == SimulationMode.Runtime)
                GUI.enabled = false;
            
            EditorGUILayout.Space();
            
            // Bake map warning
            if (emitter.simulationMode == SimulationMode.Baked && emitter.navigationTexture == null)
            {
                EditorGUILayout.HelpBox(
                    "Navigation texture not baked!\nPlease press the 'Bake Heightmap' button below.",
                    MessageType.Warning
                );
            }
            
            DrawSectionTitle("Baked Navigation Settings");
            DrawSection(() =>
            {
                
                EditorGUILayout.PropertyField(navigationTexture, new GUIContent("Navigation Texture"));

                if (navigationTexture.objectReferenceValue != null)
                {
                    Texture2D texture = navigationTexture.objectReferenceValue as Texture2D;
                    if (texture)
                    {
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        GUILayout.FlexibleSpace();
                        GUILayout.BeginVertical();
                        GUILayout.Label(texture, GUILayout.Width(100), GUILayout.Height(100));
                        GUILayout.Label(
                            $"{emitter.navigationTexture.width}x{emitter.navigationTexture.height}");
                        GUILayout.EndVertical();
                        GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                }

                EditorGUILayout.PropertyField(bakeResolutions, new GUIContent("Bake Resolutions"));
            });


            GUILayout.Space(16);

            if (GUILayout.Button("Bake Heightmap"))
            {
                foreach (var obj in serializedObject.targetObjects)
                {
                    InsectEmitter emitterObj = (InsectEmitter)obj;
                    Texture2D bakedTexture = InsectNavigationBake.Bake(emitterObj);

                    string projectDirectory = System.IO.Path.GetDirectoryName(Application.dataPath);

                    string path = UnityEditor.EditorUtility.SaveFilePanel(
                        "Save Heightmap",
                        projectDirectory + "/Assets", 
                        emitterObj.name + "_Heightmap",
                        "png"
                    );

                    if (!string.IsNullOrEmpty(path))
                    {
                        byte[] pngData = bakedTexture.EncodeToPNG();
                        System.IO.File.WriteAllBytes(path, pngData);

                        string newDirectory = System.IO.Path.GetDirectoryName(path);
                        UnityEditor.EditorPrefs.SetString("LastBakeDirectory", newDirectory);

                        if (path.StartsWith(Application.dataPath))
                        {
                            string relativePath = "Assets" + path.Substring(Application.dataPath.Length);
                            UnityEditor.AssetDatabase.ImportAsset(relativePath);
                            
                            TextureImporter importer = (TextureImporter)UnityEditor.AssetImporter.GetAtPath(relativePath);
                            if (importer)
                            {
                                importer.isReadable = true;
                                importer.SaveAndReimport();
                            }
                            
                            Texture2D savedTexture = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(relativePath);

                            emitterObj.navigationTexture = savedTexture;
                            UnityEditor.EditorUtility.SetDirty(emitterObj);
                        }
                    }
                }
            }


            GUI.enabled = true;

            GUILayout.Space(8);

            GUI.enabled = Application.isPlaying;

            if (GUILayout.Button("Start Simulation"))
            {
                foreach (var obj in serializedObject.targetObjects)
                {
                    InsectEmitter emitterObj = (InsectEmitter)obj;
                    emitterObj.StartSimulation();
                }
            }

            if (GUILayout.Button("Restart Simulation"))
            {
                foreach (var obj in serializedObject.targetObjects)
                {
                    InsectEmitter emitterObj = (InsectEmitter)obj;
                    emitterObj.RestartSimulation();
                }
            }

            if (GUILayout.Button("End Simulation"))
            {
                foreach (var obj in serializedObject.targetObjects)
                {
                    InsectEmitter emitterObj = (InsectEmitter)obj;
                    emitterObj.EndSimulation();
                }
            }

            GUI.enabled = true;

            GUI.enabled = false;
            GUILayout.Label($"Emitter State: ({emitter.emitterState.ToString()})");
            GUI.enabled = true;

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSectionTitle(string title)
        {
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
        }

        private void DrawSection(System.Action content)
        {
            Rect rect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(rect, new Color(0.20f, 0.20f, 0.20f, 1f));
            GUILayout.Space(8);
            content();
            GUILayout.Space(8);
            EditorGUILayout.EndVertical();
        }

        public void DrawHeader()
        {
            
            GUIStyle titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };

            Rect titleRect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(titleRect, new Color(0, 0.67f, 1, 1f));
            GUILayout.Space(10);
            GUILayout.Label("Insect VFX", titleStyle);
            GUILayout.Space(10);
            EditorGUILayout.EndVertical();
        }
    }
#endif
}