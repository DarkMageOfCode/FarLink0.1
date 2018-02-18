using System;
using FarLink.Logging;
using RabbitLink.Logging;

namespace FarLink.RabbitMq.Utilites
{
    internal class LinkLogFactory : ILinkLoggerFactory
    {
        private const string RabbitLinkCategoryPropertyName = "RabbitLinkCategory";
        
        private readonly ILog _logger;

        public LinkLogFactory(ILog logger)
        {
            _logger = logger;
        }


        public ILinkLogger CreateLogger(string name)
        {
            return new LinkLogger(_logger.With(RabbitLinkCategoryPropertyName, name));
        }
        
        private class LinkLogger : ILinkLogger
        {
            private readonly ILog _logger;

            public LinkLogger(ILog logger)
            {
                _logger = logger;
            }

            public void Write(LinkLoggerLevel level, string message)
            {
                switch (level)
                {
                    case LinkLoggerLevel.Error:
                        _logger.Error(message);
                        break;
                    case LinkLoggerLevel.Warning:
                        _logger.Warn(message);
                        break;
                    case LinkLoggerLevel.Info:
                        _logger.Info(message);
                        break;
                    case LinkLoggerLevel.Debug:
                        _logger.Debug(message);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(level), level, null);
                }
            }

            public void Dispose()
            {
                
            }
        }
    }
}