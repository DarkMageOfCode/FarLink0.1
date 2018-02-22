using System;

namespace FarLink.RabbitMq.Builders
{
    public interface IRabbitConfigBuilder
    {
        IRabbitConfigBuilder AppId(string appId);
        IRabbitConfigBuilder ConnectionName(string connectionName);
        IRabbitConfigBuilder RecoveryInterval(TimeSpan recoveryInterval);
        IRabbitConfigBuilder RecoveryInterval(int recoveryInterval);
        IRabbitConfigBuilder Timeout(TimeSpan timeout);
        IRabbitConfigBuilder Timeout(int timeout);
        IRabbitConfigBuilder Uri(Uri uri);
        IRabbitConfigBuilder Uri(string uri);
        IRabbitConfigBuilder UseBackgroundThreadsForConnection(bool useBackgroundThread);
    }
}