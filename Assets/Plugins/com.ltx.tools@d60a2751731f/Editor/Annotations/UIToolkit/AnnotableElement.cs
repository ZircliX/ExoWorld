using UnityEditor;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor.Annotations.UIToolkit
{
    public class AnnotableElement : BaseAnnotableElement
    {
        private const string UXML_PATH = "Packages/com.ltx.tools/Editor/Annotations/UIToolkit/AnnotableUXML.uxml";

        public new class UxmlFactory : UxmlFactory<AnnotableElement, UxmlTraits> { }
        public new class UxmlTraits : BindableElement.UxmlTraits { }

        public AnnotableElement(SerializedProperty property) : base(property, UXML_PATH)
        {

        }

        public AnnotableElement() : base(UXML_PATH)
        {
        }
    }
}