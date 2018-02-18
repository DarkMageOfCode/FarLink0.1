
using System;

namespace FarLink.Serialization
{
    public static class Extensions
    {
        public static T Deserialize<T>(this ISerializer serializer, Serialized serialized)
        {
            var obj = serializer.Deserialize(serialized, typeof(T), typeof(Exception));
            return obj is Exception ex ? throw ex : (T) obj;
        }
    }
}