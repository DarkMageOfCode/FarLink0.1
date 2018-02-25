using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Net.Mime;

namespace FarLink.Serialization
{
    public class SerializationService : ISerializationService
    {
        private readonly ITypeEncodingService _typeEncoding;
        private readonly ImmutableList<ISerializer> _serializers;
        private readonly ContentType _defaultContentType = new ContentType("application/json; charset=utf-8");

        public SerializationService(IEnumerable<ISerializer> serailizers, ITypeEncodingService typeEncoding)
        {
            if (serailizers == null) throw new ArgumentNullException(nameof(serailizers));
            _typeEncoding = typeEncoding;
            _serializers = ImmutableList.CreateRange(serailizers);
        }


        public Serialized Serialize(object value, IImmutableSet<string> skip, ContentType contentType,
            params ContentType[] alternativeContentTypes)
        {
            var serializer = new[] {contentType}
                .Union(alternativeContentTypes)
                .Select(p => _serializers.FirstOrDefault(s => s.SupportContentType(p)))
                .FirstOrDefault(p => p != null);
            if(serializer == null)
                throw new UnknownContentTypeException(contentType);
            var (body, ct)  = serializer.Serialize(value, skip, contentType);
            return new Serialized(body, ct, value != null ? _typeEncoding.EncodeType(value.GetType()) : null);
        }

        public object Deserialize(Serialized data, IImmutableDictionary<string, object> enrich, Type awaitedType, params Type[] alternativeTypes)
        {
            var contentType = data.ContentType ?? _defaultContentType;
            var serializer = _serializers.FirstOrDefault(s => s.SupportContentType(contentType));
            if(serializer == null)
                throw new UnknownContentTypeException(contentType);
            if (data.TypeCode == null)
            {
                return serializer.Deserialize(data, enrich, awaitedType);
            }

            var type = _typeEncoding.DecodeType(data.TypeCode);
            if (type != null)
                return serializer.Deserialize(data, enrich, awaitedType);
            foreach (var atype in new[] {awaitedType}.Union(alternativeTypes))
            {
                if (_typeEncoding.EncodeType(atype) == data.TypeCode)
                    return serializer.Deserialize(data, enrich, atype);
            }
            return serializer.Deserialize(data, enrich, awaitedType);
        }
    }
}