using System;
using Microsoft.Extensions.Logging;
using RabbitLink.Logging;

namespace FarLink.RabbitMq.Utilites
{
    internal class LinkLogFactory : ILinkLoggerFactory
    {
        private readonly ILoggerFactory _factory;
        private const string RabbitLinkCategoryPropertyName = "RabbitLinkCategory";
        
        

        public LinkLogFactory(ILoggerFactory factory)
        {
            _factory = factory;
        }


        public ILinkLogger CreateLogger(string name)
        {
            
            return new LinkLogger(_factory.CreateLogger<LinkLogFactory>(), name);
        }
        
        private class LinkLogger : ILinkLogger
        {
            private readonly ILogger _logger;
            private readonly string _category;

            public LinkLogger(ILogger logger, string category)
            {
                _logger = logger;
                _category = category;
            }

            public void Write(LinkLoggerLevel level, string message)
            {
                using(_logger.BeginScope($"{{{RabbitLinkCategoryPropertyName}}}", _category))
                switch (level)
                {
                    case LinkLoggerLevel.Error:
                        _logger.LogError(message);
                        break;
                    case LinkLoggerLevel.Warning:
                        _logger.LogWarning(message);
                        break;
                    case LinkLoggerLevel.Info:
                        _logger.LogInformation(message);
                        break;
                    case LinkLoggerLevel.Debug:
                        _logger.LogDebug(message);
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