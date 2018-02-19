using System;
using System.Threading;
using System.Threading.Tasks;
using FarLink.Markup;

namespace FarLink.Eventing
{
    public interface IEventPublisher<in T> 
        where T : IEvent
    {
        Task PublishAsync(T message, CancellationToken cancellation = default);
        IEventPublisher<T> MessageId(string messageId);
        IEventPublisher<T>  CorrelationId(string correlationId);
        IEventPublisher<T>  Persistent(bool persistent);
        IEventPublisher<T> ConfirmPublish(bool confirmPublish);
        IEventPublisher<T> MessageTtl(TimeSpan? messageTtl);
        IEventPublisher<T> Timeout(TimeSpan timeout);
    }
}