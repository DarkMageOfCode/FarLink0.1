using System.Threading.Tasks;
using FarLink.Markup;

namespace FarLink.Eventing
{
    public interface IEventSubscriber<TEvent> 
        where TEvent : IEvent
    {
        Task Handle(TEvent @event);
    }
}