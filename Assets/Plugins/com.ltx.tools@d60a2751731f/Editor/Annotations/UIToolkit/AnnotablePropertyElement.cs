using UnityEditor;
using UnityEngine.UIElements;

namespace LTX.Tools.Editor.Annotations.UIToolkit
{
    public class AnnotablePropertyElement : BaseAnnotableElement
    {
        private const string UXML_PATH = "Packages/com.ltx.tools/Editor/Annotations/UIToolkit/GenericAnnotableUXML.uxml";

        public new class UxmlFactory : UxmlFactory<AnnotableElement, UxmlTraits> { }
        public new class UxmlTraits : BindableElement.UxmlTraits { }

        public AnnotablePropertyElement(SerializedProperty property) : base(property, UXML_PATH)
        {

        }

        public AnnotablePropertyElement() : base(UXML_PATH)
        {
        }
    }
}