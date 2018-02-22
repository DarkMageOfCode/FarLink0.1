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
        private Action<IServiceCollection> _configureServices = sc => { };

        public IFarLink UseTypeEncoding(Action<ITypeEncodingBuilder> configure)
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

        public IFarLink UseSerializer(Action<ISerializationBuilder> configure)
        {
            configure(_serializationBuilder);
            return this;
        }

        public IFarLink ConfigureServices(Action<IServiceCollection> configurer)
        {
            if (configurer == null) throw new ArgumentNullException(nameof(configurer));
            var old = _configureServices;
            _configureServices = sp =>
            {
                old(sp);
                configurer(sp);
            };
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
            _configureServices(collection);
            return collection;
        }
        
        
    }
}