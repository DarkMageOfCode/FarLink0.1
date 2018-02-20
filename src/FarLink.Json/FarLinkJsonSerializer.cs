using System;
using System.Linq;
using System.Net.Mime;
using System.Text;
using FarLink.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FarLink.Json
{
    public class FarLinkJsonSerializer : IPartialSerializer
    {
        private readonly JsonSerializerSettings _settings;

        public FarLinkJsonSerializer(JsonSerializerSettings settings)
        {
            _settings = settings;
        }

        public FarLinkJsonSerializer() : this(new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() })
        {
        }

        public bool SupportContentType(ContentType contentType) 
            => contentType.MediaType == "application/json" || contentType.MediaType == "text/json";

        public Serialized Serialize(object value, ContentType contentType, params ContentType[] alternativeContentTypes)
        {
            var ct = contentType;
            if (!SupportContentType(contentType))
                contentType = alternativeContentTypes.FirstOrDefault(SupportContentType);
            if(contentType == null)
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
            ct = new ContentType(contentType.ToString());
            ct.CharSet = charset;
            try
            {
                var encoding = Encoding.GetEncoding(charset);
                return new Serialized(encoding.GetBytes(text), ct, null);
            }
            catch (Exception ex)
            {
                throw new UnknownContentTypeException(ct, ex);
            }
        }

        public object Deserialize(Serialized data, Type awaitedType, params Type[] alternativeTypes)
        {
            throw new NotImplementedException();
        }
    }
}