using System;

namespace FarLink.Serialization
{
    public interface ITypeEncodingService
    {
        string EncodeType(Type type);
        Type DecodeType(string code);
    }
}