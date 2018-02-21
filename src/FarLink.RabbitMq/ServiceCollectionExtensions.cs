using System;
using FarLink.Eventing;
using FarLink.Logging;
using FarLink.Markup;
using FarLink.Metadata;
using FarLink.RabbitMq.Builders;
using FarLink.RabbitMq.Utilites;
using FarLink.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RabbitLink;

namespace FarLink.RabbitMq
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRabbitFarLink(this IServiceCollection collection,
            Action<RabbitConfigBuilder> configure = null)
        {
            var builder = new RabbitConfigBuilder();
            configure?.Invoke(builder);
            collection.AddSingleton(sp =>
                new RabbitFarLink(builder.Build(), sp.GetService<ILog<RabbitFarLink>>(), sp.GetService<IMetaInfoCache>(), sp.GetService<ISerializationService>()));
            collection.TryAddTransient<IRabbitFarLink>(sp => new RabbitFarLinkAdapter(sp.GetService<RabbitFarLink>()));
            
            return collection;
        }
        
        public static IServiceCollection AddRabbitFarLink(this IServiceCollection collection,
            string appId, Func<IServiceProvider, ILink> linkFactory = null)
        {
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(appId));
            linkFactory = linkFactory ?? (sp => sp.GetService<ILink>());
            collection.AddSingleton(sp =>
                new RabbitFarLink(linkFactory(sp), appId, sp.GetService<ILog<RabbitFarLink>>(), sp.GetService<IMetaInfoCache>(), sp.GetService<ISerializationService>()));
            collection.TryAddTransient<IRabbitFarLink>(sp => new RabbitFarLinkAdapter(sp.GetService<RabbitFarLink>()));
            
            return collection;
        }

        public static IServiceCollection AddEvent<TEvent>(this IServiceCollection collection) where TEvent : IEvent
        {
            collection.AddTransient(sp =>
                sp.GetService<RabbitFarLink>().MakePublisher<TEvent>());
            return collection;
        }
        
    }
}