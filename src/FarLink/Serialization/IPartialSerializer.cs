using System;
using System.Net.Mime;

namespace FarLink.Serialization
{
    public interface IPartialSerializer : ISerializer
    {
        bool SupportContentType(ContentType contentType);
    }
}