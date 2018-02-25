using System;

namespace FarLink.RabbitMq.Builders
{
    public interface IRabbitConnectionBuilder
    {
        IRabbitConnectionBuilder AppId(string appId);
        IRabbitConnectionBuilder ConnectionName(string connectionName);
        IRabbitConnectionBuilder RecoveryInterval(TimeSpan recoveryInterval);
        IRabbitConnectionBuilder RecoveryInterval(int recoveryInterval);
        IRabbitConnectionBuilder Timeout(TimeSpan timeout);
        IRabbitConnectionBuilder Timeout(int timeout);
        IRabbitConnectionBuilder Uri(Uri uri);
        IRabbitConnectionBuilder Uri(string uri);
        IRabbitConnectionBuilder UseBackgroundThreadsForConnection(bool useBackgroundThread);
    }
}