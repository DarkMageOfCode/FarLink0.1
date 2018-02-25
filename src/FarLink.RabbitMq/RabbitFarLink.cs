using System;
using System.Collections.Immutable;
using System.Net.Mime;
using System.Reflection;
using System.Threading;
using FarLink.Eventing;
using FarLink.Markup;
using FarLink.Markup.RabbitMq;
using FarLink.Metadata;
using FarLink.RabbitMq.Configuration;
using FarLink.RabbitMq.Internals;
using FarLink.RabbitMq.Utilites;
using FarLink.Serialization;
using Microsoft.Extensions.Logging;
using RabbitLink;
using RabbitLink.Consumer;
using RabbitLink.Messaging;

namespace FarLink.RabbitMq
{
    internal class RabbitFarLink : IRabbitFarLink, IDisposable, IEventTransport
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISerializationService _serializationService;
        private readonly IMetadataCache _infoCache;
        public ILogger Logger { get; }
        public ILink Link { get; }
        public string AppId { get; }
        
        
        private readonly ProducerDictionary _producerDictionary;
        private readonly bool _ownLink;

        // ReSharper disable once SuggestBaseTypeForParameter
        public RabbitFarLink(RabbitConfig config, ILoggerFactory loggerFactory, IMetadataCache infoCache, ISerializationService serializationService)
            : this(LinkBuilder
                .Configure
                .AppId(config.AppId)
                .AutoStart(true)
                .ConnectionName(config.ConnectionName)
                .LoggerFactory(new LinkLogFactory(loggerFactory))
                .RecoveryInterval(config.RecoveryInterval)
                .Timeout(config.Timeout)
                .Uri(config.Uri)
                .UseBackgroundThreadsForConnection(config.UseBackgroundThreadsForConnection)
                .Build(), config.AppId, loggerFactory, infoCache, serializationService)
        {
            
            
        }

        public RabbitFarLink(ILink link, string appId, ILoggerFactory loggerFactory, IMetadataCache infoCache, ISerializationService serializationService)
        {
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(appId));
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _infoCache = infoCache ?? throw new ArgumentNullException(nameof(infoCache));
            _serializationService = serializationService;
            Logger = loggerFactory.CreateLogger<RabbitFarLink>();
            Link = link;
            AppId = appId;
            _producerDictionary = new ProducerDictionary(Link);
            _ownLink = false;
        }

        public IEventPublisher<TEvent> MakePublisher<TEvent>() where TEvent : IEvent
        {
            Logger.LogTrace("Making publisher for event type {EventType}", typeof(TEvent));
            return new EventPublisher<TEvent>(_loggerFactory.CreateLogger<EventPublisher<TEvent>>(), _producerDictionary, _serializationService, _infoCache, true, null, LinkDeliveryMode.Persistent,
                Timeout.InfiniteTimeSpan, null, null);
        }

        public object MakePublisher(Type eventType)
        {
            Logger.LogTrace("Making publisher for event type {EventType}", eventType);
            var publisherType = typeof(EventPublisher<>).MakeGenericType(eventType);

            return Activator.CreateInstance(publisherType,_loggerFactory.CreateLogger(eventType.FullName),  _producerDictionary, _serializationService, _infoCache, true,
                null, LinkDeliveryMode.Persistent, Timeout.InfiniteTimeSpan, null, null);
        }

        public IDisposable Subscribe<T>(IEventTransportSubscriber<T> subscriber, MethodInfo methodInfo)
        {
            using (Logger.BeginScope("{EventType} {SubscriberType} {MethodName}", typeof(T), methodInfo.ReflectedType, methodInfo.Name))
            {

                if (!Included(typeof(T), methodInfo)) return null;
                var exchange = _infoCache.GetEventAttribute<ExchangeAttribute>(typeof(T));

                if (exchange == null)
                {
                        Logger.LogWarning("Exchange attribute for {EventType} `not found");
                    return null;
                }

                string routingKey = null;
                if (exchange.Kind != ExchangeKind.Fanout)
                {
                    routingKey = _infoCache.GetEventAttribute<RoutingKeyAttribute>(typeof(T))?.Key;
                    if (routingKey == null)
                    {

                        Logger.LogError("Routing key attribute not found");
                        throw new MissingMetadataException($"No routing key for {typeof(T)} specified");
                    }
                }

                var queue = _infoCache.GetMethodAttribute<QueueAttribute>(methodInfo);
                if (queue == null)
                {
                    Logger.LogError("Queue for {EventType} on {SubscriberType}.{MethodName} bot specified");
                    
                    throw new MissingMetadataException($"No routing key for {typeof(T)} not specified");
                }

                return Link
                    .Consumer
                    .Queue(async cfg =>
                    {
                        var exch = exchange.Name == ""
                            ? await cfg.ExchangeDeclareDefault()
                            : await cfg.ExchangeDeclare(exchange.Name,
                                exchange.Kind.ToLink(), exchange.Durable, exchange.AutoDelete);
                        var que = await cfg.QueueDeclare(queue.Name, queue.Durable);
                        if (exchange.Kind == ExchangeKind.Fanout)
                            await cfg.Bind(que, exch);
                        else
                            await cfg.Bind(que, exch, routingKey);
                        return que;

                    })
                    .Handler<byte[]>(async msg =>
                    {
                        try
                        {
                            _serializationService.Deserialize(
                                new Serialized(msg.Body, new ContentType(msg.Properties.ContentType),
                                    msg.Properties.Type),
                                ImmutableDictionary<string, object>.Empty,
                                typeof(T));

                        }
                        catch (Exception ex)
                        {
                            Logger.LogError(0, ex, "Error handle message");
                            return LinkConsumerAckStrategy.Nack;
                        }

                        
                        
                        return LinkConsumerAckStrategy.Ack;
                    })
                    .Build();
                    


                

            }

        }

        private bool Included(Type eventType, MethodInfo methodInfo)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _producerDictionary.Dispose();
            if(_ownLink)
                Link.Dispose();
        }
    }
}