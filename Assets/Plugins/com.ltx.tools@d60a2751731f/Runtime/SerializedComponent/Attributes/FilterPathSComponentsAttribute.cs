using System;

namespace LTX.Tools.SerializedComponent
{
    public class FilterPathSComponentsAttribute :  Attribute
    {
        public readonly string pathConstraint;

        public FilterPathSComponentsAttribute(string pathConstraint)
        {
            this.pathConstraint = pathConstraint;
        }
    }
}