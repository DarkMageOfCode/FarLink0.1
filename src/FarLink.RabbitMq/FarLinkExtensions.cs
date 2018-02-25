using System;
using FarLink.Eventing;
using FarLink.Markup;
using FarLink.Markup.RabbitMq;
using FarLink.Metadata;
using FarLink.RabbitMq.Builders;
using FarLink.RabbitMq.Utilites;
using FarLink.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using RabbitLink;
using RabbitLink.Topology;

namespace FarLink.RabbitMq
{
    public static class FarLinkExtensions
    {
        internal static LinkExchangeType ToLink(this ExchangeKind kind)
        {
            switch (kind)
            {
                case ExchangeKind.Fanout:
                    return LinkExchangeType.Fanout;
                case ExchangeKind.Direct:
                    return LinkExchangeType.Direct;
                default:
                    throw new ArgumentOutOfRangeException(nameof(kind), kind, null);
            }
        }
        
        public static IFarLink AddRabbitMq(this IFarLink farLink, Action<IRabbitFarLinkBuilder> builder)
            => farLink.AddRabbitMq(null, builder);
        
        public static IFarLink AddRabbitMq(this IFarLink farLink, Action<IRabbitConnectionBuilder> configureConnection, Action<IRabbitFarLinkBuilder> builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            farLink.ConfigureServices(collection =>
            {
                var cfgBuilder = new RabbitConnectionBuilder();
                configureConnection?.Invoke(cfgBuilder);
                collection.AddSingleton(sp =>
                    new RabbitFarLink(cfgBuilder.Build(), sp.GetService<ILoggerFactory>(),
                        sp.GetService<IMetadataCache>(), sp.GetService<ISerializationService>()));
                
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
                    new RabbitFarLink(configureFactory(sp), appId, sp.GetService<ILoggerFactory>(),
                        sp.GetService<IMetadataCache>(), sp.GetService<ISerializationService>()));
                
                var flBuilder =new RabbitFarLinkBuilder();
                builder(flBuilder);
                flBuilder.Build(collection);
            });
            return farLink;
        }

        
        
    }
}