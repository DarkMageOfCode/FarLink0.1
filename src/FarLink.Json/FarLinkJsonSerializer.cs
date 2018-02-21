using System;
using System.Linq;
using System.Net.Mime;
using System.Text;
using FarLink.Serialization;
using Newtonsoft.Json;
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

        public (byte[], ContentType) Serialize(object value, ContentType contentType)
        {
            var ct = contentType;
            if (!SupportContentType(contentType))
                throw new UnknownContentTypeException(ct);
            string text;
            try
            {
                text = JsonConvert.SerializeObject(value, _settings);
            }
            catch (Exception ex)
            {
                throw new InvalidTypeException($"Cannot serialize {value?.GetType()}", ex);
            }

            var charset = contentType.CharSet;
            if (string.IsNullOrWhiteSpace(charset)) charset = "utf-8";
            ct = new ContentType(contentType.ToString()) {CharSet = charset};
            try
            {
                var encoding = Encoding.GetEncoding(charset);
                return (encoding.GetBytes(text), ct);
            }
            catch (Exception ex)
            {
                throw new UnknownContentTypeException(ct, ex);
            }
        }

        public object Deserialize(Serialized data, Type awaitedType)
        {
            throw new NotImplementedException();
        }
    }
}