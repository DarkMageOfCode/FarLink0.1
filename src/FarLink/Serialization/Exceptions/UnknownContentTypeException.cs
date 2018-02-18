using System;
using System.Net.Mime;
using System.Runtime.Serialization;

namespace FarLink.Serialization
{
    [Serializable]
    public class UnknownContentTypeException : SerializationException
    {
        public UnknownContentTypeException(ContentType contentType) 
            : this(contentType, $"Unknown content type {contentType}")
        {
            ContentType = contentType;
        }
        
        public UnknownContentTypeException(ContentType contentType, string message) 
            : base(message)
        {
            ContentType = contentType;
        }

        public UnknownContentTypeException(ContentType contentType, Exception innerException) 
            : this(contentType, $"Unknown content type {contentType}", innerException)
        {
        }

        public UnknownContentTypeException(ContentType contentType, string message, Exception innerException) 
            : base(message, innerException)
        {
            ContentType = contentType;
        }

        public UnknownContentTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            ContentType = new ContentType(info.GetString(nameof(ContentType)));            
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ContentType), ContentType.ToString());
        }

        public ContentType ContentType { get;  }

        internal override void Locker() { }
    }
}