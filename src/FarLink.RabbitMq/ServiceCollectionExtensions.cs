using System;
using FarLink.Logging;
using FarLink.RabbitMq.Builders;
using FarLink.RabbitMq.Utilites;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
                new RabbitFarLink(builder.Build(), sp.GetService<ILog<RabbitFarLink>>()));
            collection.TryAddTransient<IRabbitFarLink>(sp => new RabbitFarLinkAdapter(sp.GetService<RabbitFarLink>()));
            
            return collection;
        }
        
    }
}