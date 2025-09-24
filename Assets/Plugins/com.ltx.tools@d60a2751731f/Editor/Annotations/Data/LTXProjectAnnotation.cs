using UnityEngine;

namespace LTX.Tools.Editor.Annotations.Data
{
    [System.Serializable]
    public struct LTXProjectAnnotation
    {
        [SerializeField]
        public Annotable annotation;

        [SerializeField]
        public string guid;
    }
}