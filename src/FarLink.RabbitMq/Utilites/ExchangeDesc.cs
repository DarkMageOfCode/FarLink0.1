using System;
using FarLink.Markup.RabbitMq;
using RabbitLink.Topology;

namespace FarLink.RabbitMq.Utilites
{
    internal sealed class ExchangeDesc
    {
        public ExchangeDesc(string name, LinkExchangeType type, bool durable, bool autoDelete, bool delayed,
            string alternate)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type;
            Durable = durable;
            AutoDelete = autoDelete;
            Delayed = delayed;
            Alternate = alternate;
        }

        public ExchangeDesc(ExchangeAttribute attr)
        {
            if (attr == null) throw new ArgumentNullException(nameof(attr));
            Name = attr.Name;
            switch (attr.Kind)
            {
                case ExchangeKind.Fanout:
                    Type = LinkExchangeType.Fanout;
                    break;
                case ExchangeKind.Direct:
                    Type = LinkExchangeType.Direct;
                    break;
                /*case ExchangeKind.Topic:
                    Type = LinkExchangeType.Topic;
                    break;*/
                default:
                    throw new ArgumentOutOfRangeException();
            }

            Durable = attr.Durable;
            AutoDelete = attr.AutoDelete;
            Delayed = false;
            Alternate = null;

        }

        public string Name { get; }
        public LinkExchangeType Type { get; }
        public bool Durable { get; }
        public bool AutoDelete { get; }
        public bool Delayed { get; }
        public string Alternate { get; }

        private bool Equals(ExchangeDesc other)
        {
            if (string.Equals(Name, other.Name, StringComparison.InvariantCulture) && Name == "")
                return true;
            return string.Equals(Name, other.Name, StringComparison.InvariantCulture) && Type == other.Type &&
                   Durable == other.Durable && AutoDelete == other.AutoDelete && Delayed == other.Delayed &&
                   string.Equals(Alternate, other.Alternate, StringComparison.InvariantCulture);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is ExchangeDesc desc && Equals(desc);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = StringComparer.InvariantCulture.GetHashCode(Name);
                if (Name == "") return hashCode;
                hashCode = (hashCode * 397) ^ (int) Type;
                hashCode = (hashCode * 397) ^ Durable.GetHashCode();
                hashCode = (hashCode * 397) ^ AutoDelete.GetHashCode();
                hashCode = (hashCode * 397) ^ Delayed.GetHashCode();
                hashCode = (hashCode * 397) ^
                           (Alternate != null ? StringComparer.InvariantCulture.GetHashCode(Alternate) : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ExchangeDesc left, ExchangeDesc right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ExchangeDesc left, ExchangeDesc right)
        {
            return !Equals(left, right);
        }
    }
}