using System;
using System.Runtime.Serialization;

namespace FarLink.Serialization
{
    [Serializable]
    public abstract class SerializationException : Exception
    {
        protected SerializationException()
        {
        }

        protected SerializationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        protected SerializationException(string message) : base(message)
        {
        }

        protected SerializationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal abstract void Locker();
    }
}