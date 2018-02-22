using System;

namespace FarLink.Serialization
{
    public interface ITypeEncodingBuilder
    {
        ITypeEncodingBuilder Clear();
        ITypeEncodingBuilder Add(Func<IServiceProvider, ITypeEncoding> encodingFactory);
        ITypeEncodingBuilder Strict(bool strict);
    }
}