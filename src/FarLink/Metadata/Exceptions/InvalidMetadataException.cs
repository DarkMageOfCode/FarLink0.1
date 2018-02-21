using System;

namespace FarLink.Metadata
{
    public class InvalidMetadataException : Exception
    {
        public InvalidMetadataException(string message) : base(message)
        {
        }

        public InvalidMetadataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}