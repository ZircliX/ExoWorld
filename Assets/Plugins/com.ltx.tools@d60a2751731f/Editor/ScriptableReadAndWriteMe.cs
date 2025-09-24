using LTX.Tools;
using UnityEngine;
using UnityEngine.Serialization;

namespace LTX.Editor
{
    [CreateAssetMenu(menuName = "LTX/ReadAndWriteMe", fileName = "New R&W Me")]
    public class ScriptableReadAndWriteMe : ScriptableObject
    {
        [FormerlySerializedAs("readMeText")]
        [SerializeField, InspectorName("")]
        private Annotable rwm;
    }
}