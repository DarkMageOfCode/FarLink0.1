using System;
using System.Collections.Concurrent;
using System.Threading;
using FarLink.Markup.RabbitMq;
using RabbitLink;
using RabbitLink.Producer;

namespace FarLink.RabbitMq.Utilites
{
    internal class ProducerDictionary : IDisposable
    {
        private readonly ILink _link;
        private int _disposed;
        private int _usageCount;

        public ProducerDictionary(ILink link)
        {
            _link = link;
        }

        private readonly ConcurrentDictionary<(string, bool), (ExchangeAttribute, ILinkProducer)> _dictionary =
            new ConcurrentDictionary<(string, bool), (ExchangeAttribute, ILinkProducer)>();

        public bool Disposed => Interlocked.CompareExchange(ref _disposed, 0, 0) != 0;
        
        public ILinkProducer GetOrAdd(ExchangeAttribute desc, bool confirmsMode, bool passive)
        {
            if(Disposed) 
                throw new ObjectDisposedException(nameof(ProducerDictionary));
            Interlocked.Increment(ref _usageCount);
            try
            {
                if(Disposed) 
                    throw new ObjectDisposedException(nameof(ProducerDictionary));
                ILinkProducer newProducer = null;
                var (curDesc, producer) = _dictionary.GetOrAdd((desc.Name, confirmsMode), _ =>
                {
                    newProducer = _link.Producer
                        .Exchange(cfg => desc.Name == ""
                            ? cfg.ExchangeDeclareDefault()
                            : passive
                                ? cfg.ExchangeDeclarePassive(desc.Name)
                                : cfg.ExchangeDeclare(desc.Name, 
                                    desc.Kind.ToLink(), desc.Durable, desc.AutoDelete))
                        .ConfirmsMode(confirmsMode)
                        .Build();
                    return (desc, newProducer);
                });
                if (producer != newProducer)
                    newProducer?.Dispose();
                if (curDesc != desc)
                    throw new ArgumentException("Try create exchange with different parameters");
                return producer;
            }
            finally
            {
                Interlocked.Decrement(ref _usageCount);
            }
        }

        public void Dispose()
        {
            if(Interlocked.CompareExchange(ref _disposed, 1, 1) == 1) return;
            SpinWait.SpinUntil(() => Interlocked.CompareExchange(ref _usageCount, 0, 0) == 0);
            foreach (var item in _dictionary)
            {
                item.Value.Item2.Dispose();
            }
            _dictionary.Clear();
        }
    }
}