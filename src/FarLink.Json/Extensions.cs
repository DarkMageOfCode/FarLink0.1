using FarLink.Serialization;
using Newtonsoft.Json;

namespace FarLink.Json
{
    public static class Extensions
    {
        public static ISerializationBuilder AddJson(this ISerializationBuilder builder,
            JsonSerializerSettings settings = null)
            => builder.Add(_ => new FarLinkJsonSerializer(settings));
    }
}