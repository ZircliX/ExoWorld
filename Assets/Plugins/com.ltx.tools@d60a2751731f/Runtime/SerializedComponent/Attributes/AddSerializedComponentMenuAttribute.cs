using System;
using UnityEngine;

namespace LTX.Tools.SerializedComponent
{
    public class AddSerializedComponentMenuAttribute : Attribute
    {
        public readonly string Path;

        public AddSerializedComponentMenuAttribute()
        {
            Path = null;
        }

        public AddSerializedComponentMenuAttribute(string path)
        {
            Path = path;
        }
    }
}