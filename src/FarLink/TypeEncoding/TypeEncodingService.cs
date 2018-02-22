using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace FarLink.Serialization
{
    public class TypeEncodingService : ITypeEncodingService
    {
        private readonly bool _isStrict;
        private readonly ImmutableList<ITypeEncoding> _encodings;

        public TypeEncodingService(IEnumerable<Func<IServiceProvider, ITypeEncoding>> factories, bool isStrict, IServiceProvider serviceProvider)
        {
            if (factories == null) throw new ArgumentNullException(nameof(factories));
            _isStrict = isStrict;
            _encodings = factories.Select(p => p(serviceProvider)).ToImmutableList();
        }

        public string EncodeType(Type type)
        {
            var result = _encodings.Select(p => p.EncodeType(type, this)).FirstOrDefault();
            if(result == null && _isStrict)
                throw new UnknownTypeCodeException($"Type code for type {type} not found");
            return result;
        }

        public Type DecodeType(string code)
        {
            return _encodings.Select(p => p.DecodeType(code, this)).FirstOrDefault();
        }
    }
}