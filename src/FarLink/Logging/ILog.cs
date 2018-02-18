using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace FarLink.Logging
{
    public interface ILog
    {
        ILog With(IDictionary<string, object> parameters);

        void Write(LogLevel level, string template, Exception exception);
    }

    public interface ILog<T> : ILog
    {
        
    }
}