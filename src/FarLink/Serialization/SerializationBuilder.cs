using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace FarLink.Serialization
{
    internal class SerializationBuilder : ISerializationBuilder
    {
        private readonly List<Func<IServiceProvider, ISerializer>> _factories = new List<Func<IServiceProvider, ISerializer>>();

        public ISerializationBuilder Clear()
        {
            _factories.Clear();
            return this;
        }

        public ISerializationBuilder Add(Func<IServiceProvider, ISerializer> factory)
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