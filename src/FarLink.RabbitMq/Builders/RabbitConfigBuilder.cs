using System;
using System.Reflection;
using FarLink.RabbitMq.Configuration;

namespace FarLink.RabbitMq.Builders
{
    public class RabbitConfigBuilder
    {
        private string _appId;
        private string _connectionName;
        private TimeSpan _recoveryInterval;
        private TimeSpan _timeout;
        private Uri _uri;
        private bool _useBackgroundThreadsForConnection;

        internal RabbitConfigBuilder()
        {
            _appId = "FarLink";
            _connectionName = Assembly.GetEntryAssembly()?.FullName ?? "FarLink";
            _recoveryInterval = TimeSpan.FromSeconds(3);
            _timeout = TimeSpan.FromSeconds(10);
            _uri = new Uri("amqp://localhost");
            _useBackgroundThreadsForConnection = true;
        }

        public RabbitConfigBuilder AppId(string appId)
        {
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(appId));
            _appId = appId;
            return this;
        }

        public RabbitConfigBuilder ConnectionName(string connectionName)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionName));
            _connectionName = connectionName;
            return this;
        }

        public RabbitConfigBuilder RecoveryInterval(TimeSpan recoveryInterval)
        {
            if(recoveryInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(recoveryInterval), recoveryInterval, "Recovery interval must be positive");
            _recoveryInterval = recoveryInterval;
            return this;
        }
        
        public RabbitConfigBuilder RecoveryInterval(int recoveryInterval)
        {
            if(recoveryInterval <= 0)
                throw new ArgumentOutOfRangeException(nameof(recoveryInterval), recoveryInterval, "Recovery interval must be positive");
            _recoveryInterval = TimeSpan.FromMilliseconds(recoveryInterval);
            return this;
        }

        public RabbitConfigBuilder Timeout(TimeSpan timeout)
        {
            if(timeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be positive");
            _timeout = timeout;
            return this;
        }
        
        public RabbitConfigBuilder Timeout(int timeout)
        {
            if(timeout <= 0)
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be positive");
            _timeout = TimeSpan.FromMilliseconds(timeout);
            return this;
        }

        public RabbitConfigBuilder Uri(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if(uri.Scheme != "amqp" || uri.Scheme != "amqps")
                throw new ArgumentException("Uri schema must be amqp", nameof(uri));
            _uri = uri;
            return this;
        }
        
        public RabbitConfigBuilder Uri(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            var ur = new Uri(uri);
            if(ur.Scheme != "amqp" || ur.Scheme != "amqps")
                throw new ArgumentException("Uri schema must be amqp", nameof(uri));
            _uri = ur;
            return this;
        }

        public RabbitConfigBuilder UseBackgroundThreadsForConnection(bool useBackgroundThread)
        {
            _useBackgroundThreadsForConnection = useBackgroundThread;
            return this;
        }

        internal RabbitConfig Build()
        {
            return new RabbitConfig(_appId, _connectionName, _recoveryInterval, _timeout, _uri,
                _useBackgroundThreadsForConnection);
        }
    }
}