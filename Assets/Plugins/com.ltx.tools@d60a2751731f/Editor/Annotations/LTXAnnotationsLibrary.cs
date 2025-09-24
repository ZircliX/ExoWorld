using System.Collections.Generic;
using System.IO;
using System.Linq;
using LTX.Tools.Editor.Annotations.Data;
using UnityEditor;
using UnityEngine;

namespace LTX.Tools.Editor.Annotations
{
    [FilePath("ProjectSettings/LTX/LTXAnnotations.asset", FilePathAttribute.Location.ProjectFolder)]
    public class LTXAnnotationsLibrary : ScriptableSingleton<LTXAnnotationsLibrary>
    {
        [SerializeField]
        private List<LTXProjectAnnotation> projectAnnotations = new();

        private readonly DynamicBuffer<LTXProjectAnnotation> buffer = new(128);
        public string ListName => nameof(projectAnnotations);

        private void OnValidate()
        {
            //Ensure folders validity
            buffer.CopyFrom(projectAnnotations);

            for (int i = 0; i < buffer.Length; i++)
            {
                LTXProjectAnnotation data = buffer[i];
                if (!IsValid(data.guid))
                    projectAnnotations.Remove(data);
            }
        }

        public void Save() => Save(true);
        internal bool Exists(string folderPath, out LTXProjectAnnotation result, out int index)
        {
            index = GetIndex(folderPath);
            if (index == -1)
            {
                result = default;
                return false;
            }

            result = projectAnnotations[index];
            return true;
        }

        internal bool Exists(string folderPath, out LTXProjectAnnotation result) => Exists(folderPath, out result, out _);

        internal bool TryGetSerializedPropertyPath(string guid, out string property)
        {
            int idx = GetIndex(guid);
            if (idx == -1)
            {
                property = default;
                return false;
            }

            property = $"foldersData.data[{idx}]";
            return true;
        }
        internal bool DeleteFolderData(string guid, out LTXProjectAnnotation[] result)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            var subFolders = AssetDatabase.GetSubFolders(path);

            List<LTXProjectAnnotation> removed = new();

            int idx;
            for (int i = 0; i < subFolders.Length; i++)
            {
                string subFolderGuid = AssetDatabase.AssetPathToGUID(subFolders[i]);
                idx = GetIndex(subFolderGuid);
                if (idx != -1)
                {
                    removed.Add(projectAnnotations[idx]);
                    projectAnnotations.RemoveAt(idx);
                }
            }

            idx = GetIndex(guid);
            if (idx != -1)
            {
                removed.Add(projectAnnotations[idx]);
                projectAnnotations.RemoveAt(idx);
            }

            result = removed.ToArray();
            Save();
            return result.Length > 0;
        }

        internal void AddFolderData(params LTXProjectAnnotation[] data)
        {
            for (int i = 0; i < data.Length; i++)
            {
                if(projectAnnotations.Any(ctx => ctx.guid == data[i].guid))
                    continue;

                projectAnnotations.Add(data[i]);
            }

            Save();
        }
        public int GetIndex(string guid)
        {
            return projectAnnotations.FindIndex(ctx => ctx.guid == guid);
        }

        public bool IsValid(string guid)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            if (!Path.HasExtension(path))
                return AssetDatabase.IsValidFolder(path);
            else
                return !string.IsNullOrEmpty(AssetDatabase.GUIDToAssetPath(guid));
        }
    }
}