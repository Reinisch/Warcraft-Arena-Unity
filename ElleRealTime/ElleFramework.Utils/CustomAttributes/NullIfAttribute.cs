using System;

namespace ElleFramework.Utils.CustomAttributes
{
    [AttributeUsage( AttributeTargets.Property, AllowMultiple = true )]
    public sealed class NullIfAttribute : Attribute
    {
        public object	NullValue { get; set; }

        public NullIfAttribute( object nullValue )
        {
            NullValue = nullValue;
        }
    }
}
