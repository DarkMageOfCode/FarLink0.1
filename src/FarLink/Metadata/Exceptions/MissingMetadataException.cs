using System;

namespace FarLink.Metadata
{
    public class MissingMetadataException : Exception
    {
        public MissingMetadataException(string message) : base(message)
        {
        }

        public MissingMetadataException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}