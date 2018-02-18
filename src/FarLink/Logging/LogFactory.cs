using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Extensions.Logging;

namespace FarLink.Logging
{
    public class LogFactory : ILogFactory
    {
        private readonly ILoggerFactory _factory;

        public LogFactory(ILoggerFactory factory)
        {
            _factory = factory;
        }

        public ILog CreateLog(string category)
            => new Log(_factory.CreateLogger(category));

        private class Log : ILog
        {
            private readonly ImmutableDictionary<string, object> _props;
            private readonly Log _log;
            private readonly ILogger _logger;

            public Log(ILogger logger)
            {
                _logger = logger;
                _log = null;
                _props = ImmutableDictionary<string, object>.Empty;
            }

            private Log(ImmutableDictionary<string, object> props, Log log, ILogger logger)
            {
                _props = props;
                _log = log;
                _logger = logger;
            }
            
            

            private ImmutableDictionary<string, object> Enrich()
                => _log == null ? _props : _log.Enrich().SetItems(_props);

            public ILog With(IDictionary<string, object> parameters)
                => new Log(ImmutableDictionary.CreateRange(parameters), this, _logger);

            public void Write(LogLevel priority, string template, Exception exception)
            {
                using (_logger.BeginScope(Enrich()))
                {
                    switch (priority)
                    {
                        case LogLevel.Trace:
                            if(exception == null)
                                _logger.LogTrace(template);
                            else
                                _logger.LogTrace(new EventId(), exception, template);
                            break;
                        case LogLevel.Debug:
                            if(exception == null)
                                _logger.LogDebug(template);
                            else
                                _logger.LogDebug(new EventId(), exception, template);
                            break;
                        case LogLevel.Information:
                            if(exception == null)
                                _logger.LogInformation(template);
                            else
                                _logger.LogInformation(new EventId(), exception, template);
                            break;
                        case LogLevel.Warning:
                            if(exception == null)
                                _logger.LogWarning(template);
                            else
                                _logger.LogWarning(new EventId(), exception, template);
                            break;
                        case LogLevel.Error:
                            if(exception == null)
                                _logger.LogError(template);
                            else
                                _logger.LogError(new EventId(), exception, template);
                            break;
                        case LogLevel.Critical:
                            if(exception == null)
                                _logger.LogCritical(template);
                            else
                                _logger.LogCritical(new EventId(), exception, template);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(priority), priority, null);
                    }
                }
            }
        }
    }
    
    
}