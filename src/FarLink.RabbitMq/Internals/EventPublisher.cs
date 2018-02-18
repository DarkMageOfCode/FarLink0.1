using System;
using System.Threading;
using System.Threading.Tasks;
using FarLink.Eventing;
using FarLink.Markup;
using FarLink.Serialization;
using RabbitLink.Messaging;
using RabbitLink.Producer;

namespace FarLink.RabbitMq.Internals
{
    public class EventPublisher<TEvent> : IEventPublisher<TEvent> 
        where TEvent : IEvent
    {
        private readonly Lazy<ILinkProducer> _lazyProducer;
        private readonly ISerializer _serializer;
        

        private ILinkPublishMessage<byte[]> MakeMessage(TEvent message)
        {
            //var serialized = _serializer.Serialize()
            throw new NotImplementedException();
        }
        
        
        public Task PublishAsync(TEvent message, CancellationToken cancellation = default)
            => _lazyProducer.Value.PublishAsync(MakeMessage(message), cancellation);
    }

    
}