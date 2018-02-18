using System;

namespace FarLink.Serialization
{
    [Serializable]
    public class InvalidDataFormatException : SerializationException
    {
        public InvalidDataFormatException(string message) : base(message)
        {
        }

        public InvalidDataFormatException(Exception innerException) : this("Invalid serialized data format", innerException)
        {
        }

        public InvalidDataFormatException(string message, Exception innerException) : base(message, innerException)
        {
        }

        internal override void Locker()
        {
        }
    }
}