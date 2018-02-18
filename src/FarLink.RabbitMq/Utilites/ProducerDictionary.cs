using System;
using System.Collections.Concurrent;
using RabbitLink;
using RabbitLink.Producer;

namespace FarLink.RabbitMq.Utilites
{
    internal class ProducerDictionary
    {
        private readonly ILink _link;

        public ProducerDictionary(ILink link)
        {
            _link = link;
        }

        private readonly ConcurrentDictionary<(string, bool), (ExchangeDesc, ILinkProducer)> _dictionary =
            new ConcurrentDictionary<(string, bool), (ExchangeDesc, ILinkProducer)>();

        public ILinkProducer GetOrAdd(ExchangeDesc desc, bool confirmsMode, bool passive)
        {
            ILinkProducer newProducer = null;
            var (curDesc, producer) = _dictionary.GetOrAdd((desc.Name, confirmsMode), _ =>
            {
                newProducer = _link.Producer
                    .Exchange(cfg => desc.Name == ""
                        ? cfg.ExchangeDeclareDefault()
                        : passive
                            ? cfg.ExchangeDeclarePassive(desc.Name)
                            : cfg.ExchangeDeclare(desc.Name, desc.Type, desc.Durable, desc.AutoDelete,
                                desc.Alternate, desc.Delayed))
                    .ConfirmsMode(confirmsMode)
                    .Build();
                return (desc, newProducer);
            });
            if(producer != newProducer)
                newProducer?.Dispose();
            if(curDesc != desc)
                throw new ArgumentException("Try create exchange with different parameters");
            return producer;
        }
    }
}