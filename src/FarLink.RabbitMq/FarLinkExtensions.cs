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
    public static class FarLinkExtensions
    {
        public static IFarLink AddRabbitMq(this IFarLink farLink, Action<IRabbitFarLinkBuilder> builder)
            => farLink.AddRabbitMq(null, builder);
        
        public static IFarLink AddRabbitMq(this IFarLink farLink, Action<IRabbitConfigBuilder> configureConnection, Action<IRabbitFarLinkBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            farLink.ConfigureServices(collection =>
            {
                var cfgBuilder = new RabbitConfigBuilder();
                configureConnection?.Invoke(cfgBuilder);
                collection.AddSingleton(sp =>
                    new RabbitFarLink(cfgBuilder.Build(), sp.GetService<ILog<RabbitFarLink>>(),
                        sp.GetService<IMetadataCache>(), sp.GetService<ISerializationService>()));
                collection.TryAddTransient<IRabbitFarLink>(sp =>
                    new RabbitFarLinkAdapter(sp.GetService<RabbitFarLink>()));
                var flBuilder =new RabbitFarLinkBuilder();
                builder(flBuilder);
                flBuilder.Build(collection);

            });
            return farLink;
        }

        public static IFarLink AddRabbitLink(this IFarLink farLink,
            string appId, Action<IRabbitFarLinkBuilder> builder)
            => farLink.AddRabbitLink(appId, null, builder);
        
        public static IFarLink AddRabbitLink(this IFarLink farLink, string appId, Func<IServiceProvider, ILink> configureFactory,
             Action<IRabbitFarLinkBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(appId));
            farLink.ConfigureServices(collection =>
            {
                
                configureFactory = configureFactory ?? (sp => sp.GetService<ILink>());
                collection.AddSingleton(sp =>
                    new RabbitFarLink(configureFactory(sp), appId, sp.GetService<ILog<RabbitFarLink>>(),
                        sp.GetService<IMetadataCache>(), sp.GetService<ISerializationService>()));
                collection.TryAddTransient<IRabbitFarLink>(sp =>
                    new RabbitFarLinkAdapter(sp.GetService<RabbitFarLink>()));
                var flBuilder =new RabbitFarLinkBuilder();
                builder(flBuilder);
                flBuilder.Build(collection);
            });
            return farLink;
        }

        
        
    }
}