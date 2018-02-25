using System;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Mime;
using System.Text;
using FarLink.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace FarLink.Json
{
    public class FarLinkJsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _settings;

        public FarLinkJsonSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings ?? new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
        }


        public bool SupportContentType(ContentType contentType) 
            => contentType.MediaType == "application/json" || contentType.MediaType == "text/json";

        public bool SupportBinding => true;

        public (byte[], ContentType) Serialize(object value, IImmutableSet<string> skip, ContentType contentType)
        {
            var ct = contentType;
            if (!SupportContentType(contentType))
                throw new UnknownContentTypeException(ct);
            JToken json;
            try
            {
                json = JToken.FromObject(value, JsonSerializer.Create(_settings));
            }
            catch (Exception ex)
            {
                throw new InvalidTypeException($"Cannot serialize {value?.GetType()}", ex);
            }

            if (json is JObject jobj)
            {
                foreach (var propName in skip)
                {
                    jobj.Remove(propName);
                }
            }
            var charset = contentType.CharSet;
            if (string.IsNullOrWhiteSpace(charset)) charset = "utf-8";
            ct = new ContentType(contentType.ToString()) {CharSet = charset};
            try
            {
                var encoding = Encoding.GetEncoding(charset);
                return (encoding.GetBytes(json.ToString(_settings.Formatting, _settings.Converters.ToArray())), ct);
            }
            catch (Exception ex)
            {
                throw new UnknownContentTypeException(ct, ex);
            }
        }

        public object Deserialize(Serialized data, IImmutableDictionary<string, object> enrich, Type awaitedType)
        {
            throw new NotImplementedException();
        }
    }
}