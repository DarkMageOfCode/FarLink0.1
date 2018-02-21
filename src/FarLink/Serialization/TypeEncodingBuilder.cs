using System;
using System.Collections.Generic;
using FarLink.Metadata;
using Microsoft.Extensions.DependencyInjection;

namespace FarLink.Serialization
{
    public class TypeEncodingBuilder
    {
        private readonly List<Func<IServiceProvider, ITypeEncoding>> _encodings = new List<Func<IServiceProvider, ITypeEncoding>>();
        private bool _strict = false;

        public TypeEncodingBuilder()
        {
            this.UseMetadata().UseExceptions();
        }


        public TypeEncodingBuilder Clear()
        {
            _encodings.Clear();
            return this;
        }

        public TypeEncodingBuilder Add(Func<IServiceProvider, ITypeEncoding> encodingFactory)
        {
            _encodings.Add(encodingFactory);
            return this;
        }

        

        public TypeEncodingBuilder Strict(bool strict)
        {
            _strict = strict;
            return this;
        }

        internal void Build(IServiceCollection collection)
        {
            collection.AddSingleton<ITypeEncodingService>(sp => new TypeEncodingService(_encodings, _strict, sp));
        }
    }
}