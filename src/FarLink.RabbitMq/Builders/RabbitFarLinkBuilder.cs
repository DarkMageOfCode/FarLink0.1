using System;
using System.Collections.Generic;
using System.Linq;
using FarLink.Eventing;
using FarLink.Markup;
using Microsoft.Extensions.DependencyInjection;

namespace FarLink.RabbitMq.Builders
{
    public class RabbitFarLinkBuilder : IRabbitFarLinkBuilder
    {
        public readonly List<Action<IServiceCollection>> _configurers = new List<Action<IServiceCollection>>();

        public IRabbitFarLinkBuilder AddPublisher<TEvent>() where TEvent : IEvent
        {
            var type = typeof(TEvent);
            if(type == typeof(IEvent))
                throw new ArgumentException($"Publisher cannot has type {nameof(IEvent)}");
            _configurers.Add(collection => collection.AddTransient(sp =>
                sp.GetService<RabbitFarLink>().MakePublisher<TEvent>()));
            if (type.IsInterface || type.IsAbstract)
            {
                foreach (var chType in type.Assembly.GetTypes().Where(p =>
                    type.IsAssignableFrom(p) && (p.IsValueType || p.IsClass && !p.IsAbstract)))
                {
                    _configurers.Add(sc => sc.AddTransient(typeof(IEventPublisher<>).MakeGenericType(chType),
                        sp => sp.GetService<RabbitFarLink>().MakePublisher(chType)));
                }
            }

            return this;
        }

        public void Build(IServiceCollection sc)
        {
            foreach (var configurer in _configurers)
            {
                configurer(sc);
            }
        }
    }

    public interface IRabbitFarLinkBuilder
    {
        IRabbitFarLinkBuilder AddPublisher<TEvent>() where TEvent : IEvent;
    }
}