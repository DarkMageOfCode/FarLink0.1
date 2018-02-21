using System;
using System.Net.Mime;

namespace FarLink.Serialization
{
    public interface ISerializer 
    {
        bool SupportContentType(ContentType contentType);
        (byte[], ContentType) Serialize(object value, ContentType contentType);
        object Deserialize(Serialized data, Type awaitedType);
    }
}