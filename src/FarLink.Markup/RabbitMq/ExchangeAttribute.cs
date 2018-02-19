using System;

namespace FarLink.Markup.RabbitMq
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method)]
    public class ExchangeAttribute : Attribute
    {
        /// <summary>
        /// Declare default exchange
        /// </summary>
        public ExchangeAttribute()
        {
            Name = "";
        }

        /// <summary>
        /// Named exchange declaration wit Direct type 
        /// </summary>
        /// <param name="name">exchange name</param>
        public ExchangeAttribute(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        
        /// <summary>
        /// Declare exchange with name and kind 
        /// </summary>
        /// <param name="name">exchange name</param>
        /// <param name="kind">exchange kind</param>
        public ExchangeAttribute(string name, ExchangeKind kind)
        {
            Kind = kind;
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// exchange kind
        /// </summary>
        public ExchangeKind Kind { get; set; } = ExchangeKind.Direct;
        /// <summary>
        /// exchange name
        /// </summary>
        public string Name { get; set; } 
        /// <summary>
        /// durabiliyty, default true
        /// </summary>
        public bool Durable { get; set; } = true;
        /// <summary>
        /// auto delete, default false
        /// </summary>
        public bool AutoDelete { get; set; } = false;
    }
}