using System;

namespace FarLink.Serialization
{
    public interface ISerializationBuilder
    {
        ISerializationBuilder Clear();
        ISerializationBuilder Add(Func<IServiceProvider, ISerializer> factory);
    }
}