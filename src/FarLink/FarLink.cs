using System;
using FarLink.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FarLink
{
    public class FarLink : IFarLink
    {
        private readonly TypeEncodingBuilder _typeEncodingBuilder = new TypeEncodingBuilder();
        private readonly SerializationBuilder _serializationBuilder = new SerializationBuilder();
        private Action<ILoggingBuilder> _loggingBuilder = builder => { }; 

        public IFarLink UseTypeEncoding(Action<TypeEncodingBuilder> configure)
        {
            configure(_typeEncodingBuilder);
            return this;
        }

        public IFarLink AddLogging(Action<ILoggingBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            var old = _loggingBuilder;
            _loggingBuilder = p =>
            {
                old(p);
                builder(p);
            };
            return this;
        }

        public IFarLink UseSerializer(Action<SerializationBuilder> configure)
        {
            configure(_serializationBuilder);
            return this;
        }

        public IServiceCollection Prepare(IServiceCollection collection = null)
        {
            collection = collection ?? new ServiceCollection();
            collection.AddLogging(_loggingBuilder);
            collection
                .UseMetadata()
                .AddFarLinkLogging();
            _typeEncodingBuilder.Build(collection);
            _serializationBuilder.Build(collection);
            return collection;
        }
        
        
    }
}