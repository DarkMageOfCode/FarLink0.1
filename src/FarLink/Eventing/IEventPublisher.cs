using System.Threading;
using System.Threading.Tasks;
using FarLink.Markup;

namespace FarLink.Eventing
{
    public interface IEventPublisher<in T> 
        where T : IEvent
    {
        Task PublishAsync(T message, CancellationToken cancellation = default);
    }
}