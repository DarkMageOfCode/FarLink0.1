
using System;
using System.Collections.Immutable;

namespace FarLink.Serialization
{
    public static class Extensions
    {
        public static T Deserialize<T>(this ISerializationService serializationService, Serialized serialized, IImmutableDictionary<string, object> enrich = null)
        {
            var obj = serializationService.Deserialize(serialized, enrich, typeof(T), typeof(Exception));
            return obj is Exception ex ? throw ex : (T) obj;
        }
    }
}