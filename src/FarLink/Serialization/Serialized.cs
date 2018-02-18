using System.Net.Mime;

namespace FarLink.Serialization
{
    public class Serialized
    {
        public Serialized(byte[] data, ContentType contentType, string typeCode)
        {
            Data = data;
            ContentType = contentType;
            TypeCode = typeCode;
        }

        public byte[] Data { get; }
        public ContentType ContentType { get; }
        public string TypeCode { get; }
    }
}