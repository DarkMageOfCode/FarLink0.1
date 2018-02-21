using System;

namespace FarLink.Serialization
{
    public class ExceptionEncoding : ITypeEncoding
    {
        private readonly string _exceptionCode;

        public ExceptionEncoding(string exceptionCode = "error")
        {
            _exceptionCode = exceptionCode;
        }

        public string EncodeType(Type type, ITypeEncodingService encoding)
        {
            return typeof(Exception).IsAssignableFrom(type) ? _exceptionCode : null;
        }

        public Type DecodeType(string code, ITypeEncodingService encoding)
        {
            return code == _exceptionCode ? typeof(Exception) : null;
        }
    }
}