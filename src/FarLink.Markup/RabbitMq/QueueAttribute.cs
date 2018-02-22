using System;

namespace FarLink.Markup.RabbitMq
{
    [AttributeUsage(AttributeTargets.Method)]
    public class QueueAttribute : Attribute
    {
        public QueueAttribute(string name, QueueKind kind = QueueKind.Separate, bool durable = true)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));
            Name = name;
            Kind = kind;
            Durable = durable;
        }

        public QueueAttribute(QueueKind kind = QueueKind.PerApplication, bool durable = true)
        {
            Name = null;
            Kind = kind;
            Durable = durable;
        }

        public string Name { get; }
        public QueueKind Kind { get; }
        public bool Durable { get; }
    }
}