using System;

namespace FarLink.Markup
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class ContractNameAttribute : Attribute
    {
        public ContractNameAttribute(string typeCode, bool isDefault = true)
        {
            if (string.IsNullOrWhiteSpace(typeCode))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(typeCode));
            TypeCode = typeCode;
            IsDefault = isDefault;
        }

        public string TypeCode { get; }
        public bool IsDefault { get; }
    }
}