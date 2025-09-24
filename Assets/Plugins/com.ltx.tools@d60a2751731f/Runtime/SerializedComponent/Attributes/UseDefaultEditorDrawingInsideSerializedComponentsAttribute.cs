using System;

namespace LTX.Tools.SerializedComponent
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class UseDefaultEditorDrawingInsideSerializedComponentsAttribute : Attribute
    {

    }
}