using System;

namespace FarLink.Serialization
{
    [Serializable]
    public class InvalidTypeException : SerializationException
    {
        public InvalidTypeException() : this("Type not support serialization")
        {
        }

        public InvalidTypeException(string message) : base(message)
        {
        }

        public InvalidTypeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal override void Locker()
        {
        }
    }
}