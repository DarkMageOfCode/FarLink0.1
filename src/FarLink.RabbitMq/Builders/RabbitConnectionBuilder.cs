﻿using System;
using System.Reflection;
using FarLink.RabbitMq.Configuration;

namespace FarLink.RabbitMq.Builders
{
    internal class RabbitConnectionBuilder : IRabbitConnectionBuilder
    {
        private string _appId;
        private string _connectionName;
        private TimeSpan _recoveryInterval;
        private TimeSpan _timeout;
        private Uri _uri;
        private bool _useBackgroundThreadsForConnection;

        internal RabbitConnectionBuilder()
        {
            _appId = "FarLink";
            _connectionName = Assembly.GetEntryAssembly()?.FullName ?? "FarLink";
            _recoveryInterval = TimeSpan.FromSeconds(3);
            _timeout = TimeSpan.FromSeconds(10);
            _uri = new Uri("amqp://localhost");
            _useBackgroundThreadsForConnection = true;
        }

        public IRabbitConnectionBuilder AppId(string appId)
        {
            if (string.IsNullOrWhiteSpace(appId))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(appId));
            _appId = appId;
            return this;
        }

        public IRabbitConnectionBuilder ConnectionName(string connectionName)
        {
            if (string.IsNullOrWhiteSpace(connectionName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(connectionName));
            _connectionName = connectionName;
            return this;
        }

        public IRabbitConnectionBuilder RecoveryInterval(TimeSpan recoveryInterval)
        {
            if(recoveryInterval <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(recoveryInterval), recoveryInterval, "Recovery interval must be positive");
            _recoveryInterval = recoveryInterval;
            return this;
        }
        
        public IRabbitConnectionBuilder RecoveryInterval(int recoveryInterval)
        {
            if(recoveryInterval <= 0)
                throw new ArgumentOutOfRangeException(nameof(recoveryInterval), recoveryInterval, "Recovery interval must be positive");
            _recoveryInterval = TimeSpan.FromMilliseconds(recoveryInterval);
            return this;
        }

        public IRabbitConnectionBuilder Timeout(TimeSpan timeout)
        {
            if(timeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be positive");
            _timeout = timeout;
            return this;
        }
        
        public IRabbitConnectionBuilder Timeout(int timeout)
        {
            if(timeout <= 0)
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, "Timeout must be positive");
            _timeout = TimeSpan.FromMilliseconds(timeout);
            return this;
        }

        public IRabbitConnectionBuilder Uri(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            if(uri.Scheme != "amqp" && uri.Scheme != "amqps")
                throw new ArgumentOutOfRangeException("Uri schema must be amqp", uri.Scheme, nameof(uri));
            _uri = uri;
            return this;
        }
        
        public IRabbitConnectionBuilder Uri(string uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));
            var ur = new Uri(uri);
            return Uri(ur);
        }

        public IRabbitConnectionBuilder UseBackgroundThreadsForConnection(bool useBackgroundThread)
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