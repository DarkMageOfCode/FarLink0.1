using System;
using System.Net.Mime;

namespace FarLink.Markup
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ContentTypeAttribute : Attribute
    {
        public ContentTypeAttribute(string contentType)
        {
            ContentType = new ContentType(contentType);
        }

        public ContentTypeAttribute(string contentType, int priority)
        {
            ContentType = new ContentType(contentType);
            Priority = priority;
        }

        public ContentType ContentType { get; }
        public int Priority { get; }
    }
}