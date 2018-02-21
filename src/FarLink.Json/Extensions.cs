using FarLink.Serialization;
using Newtonsoft.Json;

namespace FarLink.Json
{
    public static class Extensions
    {
        public static SerializationBuilder AddJson(this SerializationBuilder builder,
            JsonSerializerSettings settings = null)
            => builder.Add(_ => new FarLinkJsonSerializer(settings));
    }
}