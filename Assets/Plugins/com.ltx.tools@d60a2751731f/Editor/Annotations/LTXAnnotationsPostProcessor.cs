using System.IO;
using UnityEditor;

namespace LTX.Tools.Editor.Annotations
{
    public class LTXAnnotationsPostProcessor : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            LTXAnnotationsLibrary library = LTXAnnotationsLibrary.instance;

            for (int i = 0; i < deletedAssets.Length; i++)
            {
                string path = deletedAssets[i];
                if(string.IsNullOrEmpty(path))
                    continue;

                string extension = Path.GetExtension(path);
                if (string.IsNullOrWhiteSpace(extension))
                {
                    string guid = AssetDatabase.AssetPathToGUID(path);
                    library.DeleteFolderData(guid, out _);
                }
            }

            EditorUtility.SetDirty(library);
            AssetDatabase.SaveAssetIfDirty(library);
        }
    }
}