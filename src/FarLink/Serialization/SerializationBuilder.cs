using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace FarLink.Serialization
{
    public class SerializationBuilder
    {
        private readonly List<Func<IServiceProvider, ISerializer>> _factories = new List<Func<IServiceProvider, ISerializer>>();

        public SerializationBuilder Clear()
        {
            _factories.Clear();
            return this;
        }

        public SerializationBuilder Add(Func<IServiceProvider, ISerializer> factory)
        {
            _factories.Add(factory);
            return this;
        }

        internal void Build(IServiceCollection collection)
        {
            collection.AddSingleton<ISerializationService>(sp =>
                new SerializationService(_factories.Select(p => p(sp)), sp.GetService<ITypeEncodingService>()));
        }
    }
}