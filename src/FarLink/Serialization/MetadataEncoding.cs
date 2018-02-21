using System;
using System.Collections.Concurrent;
using System.Linq;
using FarLink.Markup;
using FarLink.Metadata;

namespace FarLink.Serialization
{
    public class MetadataEncoding : ITypeEncoding
    {
        private readonly IMetaInfoCache _meta;
        private readonly ConcurrentDictionary<string, Type> _decodeDict = new ConcurrentDictionary<string, Type>();

        public MetadataEncoding(IMetaInfoCache meta)
        {
            _meta = meta;
        }

        public string EncodeType(Type type, ITypeEncodingService encoding)
        {
            var codes = _meta.GetTypeAttributes<ContractNameAttribute>(type).ToList();
            ContractNameAttribute defAttr;
            try
            {
                defAttr = codes.SingleOrDefault(p => p.IsDefault);
                
            }
            catch (Exception ex)
            {
                throw new InvalidMetadataException($"More then one default contract attribute assigned to {type}", ex);
            }
            
            if (defAttr == null)
                try
                {
                    defAttr = codes.SingleOrDefault();
                }
                catch (Exception ex)
                {
                    throw new InvalidMetadataException($"More then one contract attribute assigned to {type} and none form then is default", ex);
                }

            if (defAttr == null) return null;
            foreach (var attribute in codes)
            {
                _decodeDict.TryAdd(attribute.TypeCode, type);
            }

            return defAttr.TypeCode;


        }

        public Type DecodeType(string code, ITypeEncodingService encoding)
        {
            return _decodeDict.TryGetValue(code, out var type) ? type : null;
        }
    }
}