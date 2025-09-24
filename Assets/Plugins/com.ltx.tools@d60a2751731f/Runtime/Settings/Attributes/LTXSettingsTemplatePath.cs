using System;

namespace LTX.Tools.Settings.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LTXSettingsTemplatePathAttribute : Attribute
    {
        public readonly string assetPath;
    }
}