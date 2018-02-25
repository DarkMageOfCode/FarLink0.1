using System;
using System.Collections.Immutable;
using System.Net.Mime;

namespace FarLink.Serialization
{
    public interface ISerializationService
    {
        Serialized Serialize(object value, IImmutableSet<string> skip, ContentType contentType, params ContentType[] alternativeContentTypes);
        object Deserialize(Serialized data, IImmutableDictionary<string, object> enrich, Type awaitedType, params Type[] alternativeTypes);
    }
}