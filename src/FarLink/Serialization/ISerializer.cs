using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Net.Mime;

namespace FarLink.Serialization
{
    public interface ISerializer 
    {
        bool SupportContentType(ContentType contentType);
        bool SupportBinding { get; }
        (byte[], ContentType) Serialize(object value, IImmutableSet<string> skip, ContentType contentType);
        object Deserialize(Serialized data, IImmutableDictionary<string, object> enrich, Type awaitedType);
    }
}