
using System;

namespace FarLink.Serialization
{
    public static class Extensions
    {
        public static T Deserialize<T>(this ISerializationService serializationService, Serialized serialized)
        {
            var obj = serializationService.Deserialize(serialized, typeof(T), typeof(Exception));
            return obj is Exception ex ? throw ex : (T) obj;
        }
    }
}