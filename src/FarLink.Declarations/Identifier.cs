using System;
using System.Text.RegularExpressions;

namespace FarLink.Declarations
{
    public class Identifier : IEquatable<Identifier>
    {
        public Identifier(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            if(!Regex.IsMatch(value))
                throw new ArgumentException($"Invalid identifier {value}", nameof(value));
            Value = value;
        }

        internal Identifier(string value, bool noCheck)
        {
            if(noCheck)
                Value = value;
            else
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if(!Regex.IsMatch(value))
                    throw new ArgumentException("Invalid identifier", nameof(value));
                Value = value;
            }
        }

        private string Value { get; }
        
        private static readonly Regex Regex = new Regex("^[A-Za-z][A-Za-z0-9_]*$");

        public static bool TryParse(string value, out Identifier identifier)
        {
            identifier = null;
            if (value == null || !Regex.IsMatch(value))
                return false;
            identifier = new Identifier(value, true);
            return true;
        }

        public bool Equals(Identifier other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || string.Equals(Value, other.Value, StringComparison.InvariantCultureIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Identifier) obj);
        }

        public override int GetHashCode() => StringComparer.InvariantCultureIgnoreCase.GetHashCode(Value);

        public static bool operator ==(Identifier left, Identifier right) => Equals(left, right);

        public static bool operator !=(Identifier left, Identifier right) => !Equals(left, right);

        public override string ToString() => Value;

    }
}