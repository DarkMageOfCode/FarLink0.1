using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace FarLink.Logging
{
    public class Log<T> : ILog<T>
    {
        private readonly ILog _log;
        
        public Log(ILogFactory factory)
        {
            _log = factory.CreateLog(typeof(T).FullName);
        }

        public ILog With(IDictionary<string, object> parameters)
            => _log.With(parameters);

        public void Write(LogLevel level, string template, Exception exception)
            => _log.Write(level, template, exception);
    }
}