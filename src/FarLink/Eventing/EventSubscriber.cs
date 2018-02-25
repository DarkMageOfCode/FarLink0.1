using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace FarLink.Eventing
{
    public abstract class EventSubscriber
    {
        protected IEventContext Context { get; }       
    }

    public interface IEventContext
    {
        string MessageId { get; }
        string CorrelationId { get; }
        string SenderId { get; }
    }

    public interface IEventTransportSubscriber<T>
    {
        Task Handle(T message, IEventContext context, CancellationToken cancellation);
    }

    public interface IEventTransport
    {
        IDisposable Subscribe<T>(IEventTransportSubscriber<T> subscriber, MethodInfo methodInfo);
    }
}