using System;
using System.Threading;
using FarLink.Eventing;
using FarLink.Logging;
using FarLink.Markup;
using FarLink.Markup.RabbitMq;
using FarLink.Metadata;
using FarLink.RabbitMq.Configuration;
using FarLink.RabbitMq.Internals;
using FarLink.RabbitMq.Utilites;
using FarLink.Serialization;
using RabbitLink;
using RabbitLink.Messaging;

namespace FarLink.RabbitMq
{
    internal class RabbitFarLink : IRabbitFarLink, IDisposable, IEventTransport
    {
        private readonly ISerializationService _serializationService;
        private readonly IMetadataCache _infoCache;
        public ILog Logger { get; }
        public ILink Link { get; }
        public string AppId { get; }
        
        
        private readonly ProducerDictionary _producerDictionary;
        private readonly bool _ownLink;

        // ReSharper disable once SuggestBaseTypeForParameter
        public RabbitFarLink(RabbitConfig config, ILog<RabbitFarLink> logger, IMetadataCache infoCache, ISerializationService serializationService)
            : this(LinkBuilder
                .Configure
                .AppId(config.AppId)
                .AutoStart(true)
                .ConnectionName(config.ConnectionName)
                .LoggerFactory(new LinkLogFactory(logger))
                .RecoveryInterval(config.RecoveryInterval)
                .Timeout(config.Timeout)
                .Uri(config.Uri)
                .UseBackgroundThreadsForConnection(config.UseBackgroundThreadsForConnection)
                .Build(), config.AppId, logger, infoCache, serializationService)
        {
            _serializationService = serializationService;
            _ownLink = true;
        }

        public RabbitFarLink(ILink link, string appId, ILog<RabbitFarLink> logger, IMetadataCache infoCache, ISerializationService serializationService)
        {
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(appId));
            _infoCache = infoCache ?? throw new ArgumentNullException(nameof(infoCache));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            Link = link;
            AppId = appId;
            _producerDictionary = new ProducerDictionary(Link);
            _ownLink = false;
        }

        public IEventPublisher<TEvent> MakePublisher<TEvent>() where TEvent : IEvent
        {
            return new EventPublisher<TEvent>(_producerDictionary, _serializationService, _infoCache, true, null, LinkDeliveryMode.Persistent,
                Timeout.InfiniteTimeSpan, null, null);
        }

        public object MakePublisher(Type eventType)
        {
            var publisherType = typeof(EventPublisher<>).MakeGenericType(eventType);

            return Activator.CreateInstance(publisherType, _producerDictionary, _serializationService, _infoCache, true,
                null, LinkDeliveryMode.Persistent, Timeout.InfiniteTimeSpan, null, null);
        }

        public IDisposable Subscribe(ITransportEventSubscriber subscriber)
        {
            if (!Included(subscriber)) return null;
            var exchange = _infoCache.GetEventAttribute<ExchangeAttribute>(subscriber.EventType);

            if (exchange == null)
            {
                Logger.With("EventType", subscriber.EventType).Warn("Exchange attribute not found");
                return null;
            }

            string routingKey;
            if (exchange.Kind != ExchangeKind.Fanout)
            {
                routingKey = _infoCache.GetEventAttribute<RoutingKeyAttribute>(subscriber.EventType)?.Key;
                if (routingKey == null)
                {
                    Logger.With("EventType", subscriber.EventType).Error("Routing key attribute not found");
                    throw new MissingMetadataException($"No routing key for {subscriber.EventType} specified");
                }
            }
            
            var queue = _infoCache.Get
            if()



        }

        private bool Included(ITransportEventSubscriber subscriber)
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