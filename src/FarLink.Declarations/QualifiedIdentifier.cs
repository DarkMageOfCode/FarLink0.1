using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace FarLink.Declarations
{
    public class QualifiedIdentifier : IEquatable<QualifiedIdentifier>
    {
        public QualifiedIdentifier(Identifier[] names)
        {
            if (names == null) throw new ArgumentNullException(nameof(names));
            if (names.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(names));
            Names = names;
        }
        
        public QualifiedIdentifier(Identifier name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            Names = new [] {name};
        }
        
        
        

        public QualifiedIdentifier(string value)
        {
            Names = ParseInternal(value);
            if(Names == null)
                throw new ArgumentException($"Invalid qualified identifier {value}", nameof(value));
        }

        public int Count => Names.Length;

        public Identifier Name => Names[Names.Length - 1];

        public Identifier[] Namespace => Names.Take(Names.Length - 1).ToArray();

        public override string ToString()
            => Names.Aggregate(new StringBuilder(), (a, id) => a.Length == 0 ? a.Append(id) : a.Append(".").Append(id),
                a => a.ToString());

        public static bool TryParse(string value, out QualifiedIdentifier identifier)
        {
            identifier = null;
            var v = ParseInternal(value);
            if (v == null || v.Length == 0) return false;
            identifier = new QualifiedIdentifier(v);
            return true;
        }

        public bool Equals(QualifiedIdentifier other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            
            return Names.Length == other.Names.Length && Names.Zip(other.Names, (a, b) => a == b).All(p => p);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((QualifiedIdentifier) obj);
        }

        public override int GetHashCode()
        {
            return Names.Aggregate(0, (a, id) => a ^ id.GetHashCode());
        }

        public static bool operator ==(QualifiedIdentifier left, QualifiedIdentifier right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(QualifiedIdentifier left, QualifiedIdentifier right)
        {
            return !Equals(left, right);
        }


        private Identifier[] Names { get; }

        private static readonly Regex Regex =
            new Regex("^(?:(?<ns>[A-Za-z][A-Za-z0-9_]*)[.])*(?<name>[A-Za-z][A-Za-z0-9_]*)$");

        private static Identifier[] ParseInternal(string value)
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            var match = Regex.Match(value);
            if (!match.Success) return null;
            return
                match.Groups["ns"].Captures
                    .Cast<Capture>()
                    .Select(p => new Identifier(p.Value, true))
                    .Union(new[] {new Identifier(match.Groups["name"].Value, true)})
                    .ToArray();
        }

        public static implicit operator QualifiedIdentifier(string value) => new QualifiedIdentifier(value);
    }
}