using System;

namespace FarLink.Serialization
{
    public interface ITypeEncoding
    {
        string EncodeType(Type type, ITypeEncodingService encoding);
        Type DecodeType(string code, ITypeEncodingService encoding);
    }

    public interface IITypeEncoder<T>
    {
        Some<string> Code { get; }
    }
}