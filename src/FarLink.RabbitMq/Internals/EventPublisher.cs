using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FarLink.Eventing;
using FarLink.Markup;
using FarLink.Markup.RabbitMq;
using FarLink.Metadata;
using FarLink.RabbitMq.Utilites;
using FarLink.Serialization;
using RabbitLink.Messaging;
using RabbitLink.Producer;

namespace FarLink.RabbitMq.Internals
{
    internal class EventPublisher<TEvent> : IEventPublisher<TEvent> 
        where TEvent : IEvent
    {
        private readonly ProducerDictionary _producers;
        private readonly ISerializer _serializer;
        private readonly IMetaInfoCache _metaInfo;
        private readonly bool _confirmsMode;
        private readonly string _correlationId;
        private readonly string _messageId;
        private readonly TimeSpan? _messageTtl;
        private readonly LinkDeliveryMode _deliveryMode;
        private readonly TimeSpan _timeout;

        public EventPublisher(ProducerDictionary producers, ISerializer serializer, IMetaInfoCache metaInfo,
            bool confirmsMode, TimeSpan? messageTtl, LinkDeliveryMode deliveryMode, TimeSpan timeout, string correlationId, string messageId)
        {
            _producers = producers ?? throw new ArgumentNullException(nameof(producers));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _metaInfo = metaInfo ?? throw new ArgumentNullException(nameof(metaInfo));
            _confirmsMode = confirmsMode;
            _messageTtl = messageTtl;
            _deliveryMode = deliveryMode;
            _timeout = timeout;
            _correlationId = correlationId;
            _messageId = messageId;
        }

        public IEventPublisher<TEvent> MessageId(string messageId)
            => new EventPublisher<TEvent>(_producers, _serializer, _metaInfo, _confirmsMode, _messageTtl, _deliveryMode,
                _timeout, _correlationId, messageId);
            

        public IEventPublisher<TEvent>  CorrelationId(string correlationId)
            => new EventPublisher<TEvent>(_producers, _serializer, _metaInfo, _confirmsMode, _messageTtl, _deliveryMode, _timeout,
                correlationId, _messageId);
        
        public IEventPublisher<TEvent>  Persistent(bool persistent)
            => new EventPublisher<TEvent>(_producers, _serializer, _metaInfo, _confirmsMode, _messageTtl, persistent ? LinkDeliveryMode.Persistent : LinkDeliveryMode.Transient, _timeout,
                _correlationId, _messageId);
        
        public IEventPublisher<TEvent> ConfirmPublish(bool confirmPublish)
            => new EventPublisher<TEvent>(_producers, _serializer, _metaInfo, confirmPublish, _messageTtl, _deliveryMode, _timeout,
                _correlationId, _messageId);

        public IEventPublisher<TEvent> MessageTtl(TimeSpan? messageTtl)
            => new EventPublisher<TEvent>(_producers, _serializer, _metaInfo, _confirmsMode,
                messageTtl > TimeSpan.Zero
                    ? messageTtl
                    : throw new ArgumentOutOfRangeException(nameof(messageTtl), messageTtl,
                        "Message Ttl must be positive value"), _deliveryMode, _timeout,
                _correlationId, _messageId);
        
        public IEventPublisher<TEvent> Timeout(TimeSpan timeout)
            => new EventPublisher<TEvent>(_producers, _serializer, _metaInfo, _confirmsMode, _messageTtl, _deliveryMode, timeout,
                _correlationId, _messageId);
        

        public async Task PublishAsync(TEvent message, CancellationToken cancellation = default)
        {
            var eventType = message?.GetType() ?? typeof(TEvent);
            var exchAttr = _metaInfo.GetEventAttribute<ExchangeAttribute>(eventType);
            if(exchAttr == null)
                throw new MissingMetadataException($"For event {eventType} attribute {typeof(ExchangeAttribute)} not found");
            string routingKey = null;
            if (exchAttr.Kind != ExchangeKind.Fanout)
            {
                var routingKeyAttr = _metaInfo.GetEventAttribute<RoutingKeyAttribute>(eventType);
                if (routingKeyAttr == null)
                    throw new MissingMetadataException(
                        $"For event {eventType} attribute {typeof(RoutingKeyAttribute)} not found");
                routingKey = routingKeyAttr.Key;
            }

            var contentTypes = _metaInfo.GetEventAttributes<ContentTypeAttribute>(eventType)?.OrderBy(p => p.Priority).ToList();
            if(contentTypes == null || contentTypes.Count == 0 )
                throw new MissingMetadataException(
                    $"For event {eventType} attribute {typeof(ContentTypeAttribute)} not found");
            var serialized = _serializer.Serialize(message, contentTypes[0].ContentType,
                contentTypes.Skip(1).Select(p => p.ContentType).ToArray());

            var producer = _producers.GetOrAdd(new ExchangeDesc(exchAttr), _confirmsMode, false);
            
            var props = new LinkMessageProperties
            {
                ContentType = serialized.ContentType.ToString(),
                Type = serialized.TypeCode,
                DeliveryMode = _deliveryMode
            };
            if (_correlationId != null)
                props.CorrelationId = _correlationId;
            if (_messageId != null)
                props.MessageId = _messageId;
            if (_messageTtl != null)
                props.Expiration = _messageTtl.Value;

            var pubProps = new LinkPublishProperties();
            if (exchAttr.Kind != ExchangeKind.Fanout)
                pubProps.RoutingKey = routingKey;

            CancellationTokenSource cts = null; 
            if (!cancellation.CanBeCanceled && _timeout > TimeSpan.Zero)
            {
                cts = new CancellationTokenSource(_timeout);
                cancellation = cts.Token;
            }

            try
            {
                await producer.PublishAsync(new LinkPublishMessage<byte[]>(serialized.Data, props, pubProps),
                    cancellation).ConfigureAwait(false);
            }
            finally
            {
                cts?.Dispose();
            }
        }
        
        
        
            
    }

    
}