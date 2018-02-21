using System;

namespace FarLink.Serialization
{
    public class UnknownTypeCodeException : Exception
    {
        public UnknownTypeCodeException(string message) : base(message)
        {
        }

        public UnknownTypeCodeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}