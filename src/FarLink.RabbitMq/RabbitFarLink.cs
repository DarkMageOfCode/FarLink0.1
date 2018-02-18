using System;
using FarLink.Logging;
using FarLink.RabbitMq.Configuration;
using FarLink.RabbitMq.Utilites;
using RabbitLink;

namespace FarLink.RabbitMq
{
    internal class RabbitFarLink : IRabbitFarLink, IDisposable
    {
        public ILog Logger { get; }
        public RabbitConfig Config { get; }
        public ILink Link { get; }

        // ReSharper disable once SuggestBaseTypeForParameter
        public RabbitFarLink(RabbitConfig config, ILog<RabbitFarLink> logger)
        {
            Logger = logger;
            Config = config;
            Link = LinkBuilder
                .Configure
                .AppId(Config.AppId)
                .AutoStart(true)
                .ConnectionName(Config.ConnectionName)
                .LoggerFactory(new LinkLogFactory(logger))
                .RecoveryInterval(Config.RecoveryInterval)
                .Timeout(Config.Timeout)
                .Uri(Config.Uri)
                .UseBackgroundThreadsForConnection(Config.UseBackgroundThreadsForConnection)
                .Build();
        }

        public void Dispose()
        {
            Link.Dispose();
        }
    }
}