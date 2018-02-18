using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace FarLink.Logging
{
    public static class Extensions
    {
        public static ILog<T> CreateLog<T>(this ILogFactory factory)
            => new Log<T>(factory);
        
        public static void Trace(this ILog log, string template, Exception ex = null)
            => log.Write(LogLevel.Trace, template, ex);
        
        public static void Trace(this ILog log, Exception ex)
            => log.Write(LogLevel.Trace, ex.Message, ex);
        
        public static void Debug(this ILog log, string template, Exception ex = null)
            => log.Write(LogLevel.Debug, template, ex);
        
        public static void Debug(this ILog log, Exception ex)
            => log.Write(LogLevel.Debug, ex.Message, ex);
        
        public static void Info(this ILog log, string template, Exception ex = null)
            => log.Write(LogLevel.Information, template, ex);
        
        public static void Info(this ILog log, Exception ex)
            => log.Write(LogLevel.Information, ex.Message, ex);
        
        public static void Warn(this ILog log, string template, Exception ex = null)
            => log.Write(LogLevel.Warning, template, ex);
        
        public static void Warn(this ILog log, Exception ex)
            => log.Write(LogLevel.Warning, ex.Message, ex);
        
        public static void Error(this ILog log, string template, Exception ex = null)
            => log.Write(LogLevel.Error, template, ex);
        
        public static void Error(this ILog log, Exception ex)
            => log.Write(LogLevel.Error, ex.Message, ex);
        
        public static void Fatal(this ILog log, string template, Exception ex = null)
            => log.Write(LogLevel.Critical, template, ex);
        
        public static void Fatal(this ILog log, Exception ex)
            => log.Write(LogLevel.Critical, ex.Message, ex);

        public static ILog With(this ILog log, string property, object value)
            => log.With(new Dictionary<string, object> { {property, value } });

        public static ILog With(this ILog log, params (string property, object value)[] properties)
            => log.With(properties.ToDictionary(p => p.property, p => p.value));

    }
}