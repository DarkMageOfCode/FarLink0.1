using System;
using RabbitLink;

namespace FarLink.RabbitMq.Configuration
{
    public class RabbitConfig
    {
        public RabbitConfig(string appId, string connectionName, TimeSpan recoveryInterval, TimeSpan timeout, Uri uri,
            bool useBackgroundThreadsForConnection)
        {
            AppId = appId;
            ConnectionName = connectionName;
            RecoveryInterval = recoveryInterval;
            Timeout = timeout;
            Uri = uri;
            UseBackgroundThreadsForConnection = useBackgroundThreadsForConnection;
        }

        public string AppId { get; }
        public string ConnectionName { get; }
        public TimeSpan RecoveryInterval { get; }
        public TimeSpan Timeout { get; }
        public Uri Uri { get; }
        public bool UseBackgroundThreadsForConnection { get; }
    }
}