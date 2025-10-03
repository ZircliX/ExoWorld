// HDRPMaskMapGenerator created by Marie Valance
// Licence: Creative Commons
// Package created by Seta and shared with the creator's permission
using UnityEngine;
using UnityEditor;
using System.IO;


namespace HDRPMaskGenerator
{
    public class HDRPMaskMapGenerator : EditorWindow
    {
        private Texture2D sourceTexture;
        private Texture2D generatedPreview;

        private Vector2 scrollPosition;

        /// <summary>
        /// Creates the Menu Dialog
        /// </summary>
        [MenuItem("Tools/HDRP/Mask Map Generator")]
        public static void ShowWindow()
        {
            GetWindow<HDRPMaskMapGenerator>("HDRP Mask Map Generator v.2");
        }

        /// <summary>
        /// Draws the GUI
        /// </summary>
        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            // --- Window Title and generell Texts ---
            EditorGUILayout.LabelField("HDRP Mask Map Generator", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "This Tool creates an Mask Map only by One Single Texture.\n\n" +
                "1. Select your wished Texture.\n" +
                "2. This tool automaticlly creates 4 Textures from it(Metallic, AO, Detail, Smoothness).\n" +
                "3. The 4 Textures are getting combined to create the Mask Map.", MessageType.Info);

            EditorGUILayout.Space(10);

            // --- Textur-Inputfield ---
            EditorGUILayout.LabelField("Source Texture", EditorStyles.boldLabel);
            sourceTexture = (Texture2D)EditorGUILayout.ObjectField("Source Texture", sourceTexture, typeof(Texture2D), false);

            EditorGUILayout.Space(20);

            // --- Action-Button ---
            GUI.backgroundColor = new Color(0.2f, 0.8f, 0.2f);
            if (GUILayout.Button("Genereate every Texture including the Mask Map", GUILayout.Height(40)))
            {
                if (ValidateInput())
                {
                    ProcessTextures();
                }
            }
            GUI.backgroundColor = Color.white;

            EditorGUILayout.Space(10);

            // --- Preshow-Section ---
            EditorGUILayout.LabelField("Preshow of the Generated Texture", EditorStyles.boldLabel);
            if (generatedPreview != null)
            {
                EditorGUI.DrawPreviewTexture(EditorGUILayout.GetControlRect(false, position.width - 40), generatedPreview);
            }
            else
            {
                EditorGUILayout.HelpBox("A Preview of the Mask Map will be shown here after generation.", MessageType.None);
            }

            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Checks if an Texture is availabel
        /// </summary>
        private bool ValidateInput()
        {
            if (sourceTexture == null)
            {
                EditorUtility.DisplayDialog("Missing Texture", "Please link the texture bevor you start...", "OK");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Main Function of transforming
        /// </summary>
        private void ProcessTextures()
        {
            if (!MakeTextureReadable(sourceTexture)) return;

            string sourcePath = AssetDatabase.GetAssetPath(sourceTexture);
            string sourceDirectory = Path.GetDirectoryName(sourcePath);
            string sourceName = Path.GetFileNameWithoutExtension(sourceTexture.name);

            EditorUtility.DisplayProgressBar("Generating Textures...", "Starting Prozess...", 0f);

            Color32[] srcPixels = sourceTexture.GetPixels32();
            int width = sourceTexture.width;
            int height = sourceTexture.height;
            int total = srcPixels.Length;


            Color32[] metallicPixels = new Color32[total];
            Color32[] aoPixels = new Color32[total];
            Color32[] detailPixels = new Color32[total];
            Color32[] smoothnessPixels = new Color32[total];
            Color32[] maskPixels = new Color32[total];

            for (int i = 0; i < total; i++)
            {
                Color32 c = srcPixels[i];
                byte grey = (byte)(0.299f * c.r + 0.587f * c.g + 0.114f * c.b);
                byte invGrey = (byte)(255 - grey);

                metallicPixels[i] = new Color32(0, 0, 0, 255);      // ca³e czarne
                aoPixels[i] = new Color32(invGrey, invGrey, invGrey, 255);
                detailPixels[i] = new Color32(grey, grey, grey, 255);
                smoothnessPixels[i] = new Color32(grey, grey, grey, 255);
                maskPixels[i] = new Color32(
                    metallicPixels[i].r, // R
                    aoPixels[i].r,       // G
                    detailPixels[i].r,   // B
                    smoothnessPixels[i].r // A
                );
            }

            Texture2D metallicTex = MakeTex(width, height, metallicPixels);
            Texture2D aoTex = MakeTex(width, height, aoPixels);
            Texture2D detailTex = MakeTex(width, height, detailPixels);
            Texture2D smoothnessTex = MakeTex(width, height, smoothnessPixels);
            Texture2D maskMapTex = MakeTex(width, height, maskPixels);
            generatedPreview = maskMapTex;
            Repaint();

            SaveTexture(metallicTex, sourceDirectory, $"{sourceName}_Metallic.png", true);
            SaveTexture(aoTex, sourceDirectory, $"{sourceName}_AO.png", true);
            SaveTexture(detailTex, sourceDirectory, $"{sourceName}_Detail.png", true);
            SaveTexture(smoothnessTex, sourceDirectory, $"{sourceName}_Smoothness.png", true);
            SaveTexture(maskMapTex, sourceDirectory, $"{sourceName}_MaskMap.png", false);

            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("Success!", $"All maps saved in:\n{sourceDirectory}", "OK");
        }

        private Texture2D MakeTex(int w, int h, Color32[] pixels)
        {
            Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
            tex.SetPixels32(pixels);
            tex.Apply(false);
            return tex;
        }

        private void SaveTexture(Texture2D tex, string dir, string filename, bool srgb)
        {
            string fullPath = Path.Combine(dir, filename);
            File.WriteAllBytes(fullPath, tex.EncodeToPNG());
            AssetDatabase.ImportAsset(fullPath);

            TextureImporter importer = AssetImporter.GetAtPath(fullPath) as TextureImporter;
            if (importer != null)
            {
                importer.sRGBTexture = srgb;
                importer.isReadable = false;
                importer.SaveAndReimport();
            }
        }

        private bool MakeTextureReadable(Texture2D texture)
        {
            if (texture.isReadable) return true;

            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
            if (importer != null)
            {
                importer.isReadable = true;
                importer.SaveAndReimport();
                AssetDatabase.Refresh();
                return true;
            }

            EditorUtility.DisplayDialog("Error", $"Could not make '{texture.name}' readable. Please Activate it Manually in the Import Settings of the Texture", "OK");
            return false;
        }

        private void OnDestroy()
        {
            if (generatedPreview != null)
            {
                DestroyImmediate(generatedPreview);
            }
        }
    }
}